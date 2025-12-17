using Dapr.Workflow;
using Microsoft.Extensions.Logging;
using Workflow.Models;

namespace Workflow.Activities;

public class ProcessDeclinedOfferActivity : WorkflowActivity<(OrderRequest Order, string Reason), OrderProcessingResult>
{
    private readonly ILogger<ProcessDeclinedOfferActivity> _logger;

    public ProcessDeclinedOfferActivity(ILogger<ProcessDeclinedOfferActivity> logger)
    {
        _logger = logger;
    }

    public override Task<OrderProcessingResult> RunAsync(WorkflowActivityContext context, (OrderRequest Order, string Reason) input)
    {
        _logger.LogInformation("Processing declined offer for order {OrderId}. Reason: {Reason}",
            input.Order.OrderId, input.Reason);

        var result = new OrderProcessingResult
        {
            OrderId = input.Order.OrderId,
            Status = "Declined",
            Message = $"Order has been declined. Reason: {input.Reason}",
            ProcessedAt = DateTime.UtcNow
        };

        _logger.LogInformation("Order {OrderId} declined and cancelled", input.Order.OrderId);

        return Task.FromResult(result);
    }
}
