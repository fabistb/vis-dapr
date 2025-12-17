using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Workflow.Models;
using Workflow.Workflows;

namespace Workflow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<OrderController> _logger;
    private readonly ILoggerFactory _loggerFactory;

    public OrderController(DaprClient daprClient, ILogger<OrderController> logger, ILoggerFactory loggerFactory)
    {
        _daprClient = daprClient;
        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
    {
        try
        {
            _logger.LogInformation("Creating order {OrderId} for customer {CustomerId}",
                request.OrderId, request.CustomerId);

            // In a real workflow scenario, you would start a workflow via Dapr
            // For this example, we'll process the order directly using the service
            var processor = new OrderProcessor(_logger, _loggerFactory);
            var result = await processor.ProcessOrderAsync(request);

            _logger.LogInformation("Order processing completed for {OrderId} with status {Status}",
                request.OrderId, result.Status);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order {OrderId}", request.OrderId);
            return StatusCode(500, new { error = "Failed to create order", details = ex.Message });
        }
    }

    [HttpPost("{orderId}/respond")]
    public async Task<IActionResult> RespondToOffer(string orderId, [FromBody] OfferResponse response)
    {
        try
        {
            _logger.LogInformation("Processing offer response for order {OrderId}: {IsAccepted}",
                orderId, response.IsAccepted);

            // In a real workflow, this would raise an event to the running workflow
            // For this example, we'll just acknowledge the response
            
            return Ok(new
            {
                orderId = orderId,
                accepted = response.IsAccepted,
                message = response.IsAccepted ? "Offer accepted" : $"Offer declined: {response.Reason}"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing offer response for order {OrderId}", orderId);
            return StatusCode(500, new { error = "Failed to process offer response", details = ex.Message });
        }
    }
}
