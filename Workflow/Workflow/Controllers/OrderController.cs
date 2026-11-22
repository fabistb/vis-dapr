using Asp.Versioning;
using Dapr.Workflow;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Workflow.Models;
using Workflow.Workflows;

namespace Workflow.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/orders")]
public class OrderController(IDaprWorkflowClient workflowClient) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Start([FromBody] OrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.OrderId))
            return BadRequest("OrderId is required");
        if (request.OrderId.EndsWith(OrderProcessingWorkflow.InventorySuffix) || request.OrderId.EndsWith(OrderProcessingWorkflow.FulfillmentSuffix))
            return BadRequest("OrderId must not end with a reserved child workflow suffix");
        if ((await workflowClient.GetWorkflowStateAsync(request.OrderId, getInputsAndOutputs: false)).Exists)
            return Conflict($"Order {request.OrderId} already exists");

        string id;
        try
        {
            id = await workflowClient.ScheduleNewWorkflowAsync(nameof(OrderProcessingWorkflow), request.OrderId, request);
        }
        // The sidecar reports a duplicate id as StatusCode.Unknown, so the detail text is the only discriminator.
        catch (RpcException e) when (e.Status.Detail.Contains("already exists", StringComparison.OrdinalIgnoreCase))
        {
            // Lost the race against a concurrent request with the same id.
            return Conflict($"Order {request.OrderId} already exists");
        }

        return AcceptedAtAction(nameof(Get), new { id, v = HttpContext.RequestedApiVersion?.ToString() }, new { id });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var state = await workflowClient.GetWorkflowStateAsync(id, getInputsAndOutputs: true);
        if (!state.Exists) return NotFound();

        return Ok(new
        {
            id,
            status = state.RuntimeStatus.ToString(),
            customStatus = state.ReadCustomStatusAs<string>(),
            output = state.RuntimeStatus == WorkflowRuntimeStatus.Completed ? state.ReadOutputAs<OrderResult>() : null,
            error = state.RuntimeStatus == WorkflowRuntimeStatus.Failed
                ? new { type = state.FailureDetails?.ErrorType, message = state.FailureDetails?.ErrorMessage }
                : null,
        });
    }

    [HttpPost("{id}/response")]
    public async Task<IActionResult> Respond(string id, [FromBody] OfferResponse response)
    {
        // Dapr silently drops events for unknown or finished workflows, so check first.
        var state = await workflowClient.GetWorkflowStateAsync(id, getInputsAndOutputs: true);
        if (!state.Exists) return NotFound();
        if (state.IsWorkflowCompleted) return Conflict($"Order {id} is already {state.RuntimeStatus}");
        if (state.ReadCustomStatusAs<string>() != OrderProcessingWorkflow.AwaitingOfferStatus)
            return Conflict($"Order {id} is not waiting for an offer response");

        await workflowClient.RaiseEventAsync(id, OrderProcessingWorkflow.OfferResponseEvent, response);
        return Accepted();
    }
}
