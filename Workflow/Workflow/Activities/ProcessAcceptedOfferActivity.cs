using Dapr.Workflow;
using Microsoft.Extensions.Logging;
using Workflow.Models;

namespace Workflow.Activities;

public class ProcessAcceptedOfferActivity : WorkflowActivity<(OrderRequest Order, PriceCalculation Price), OrderProcessingResult>
{
    private readonly ILogger<ProcessAcceptedOfferActivity> _logger;

    public ProcessAcceptedOfferActivity(ILogger<ProcessAcceptedOfferActivity> logger)
    {
        _logger = logger;
    }

    public override Task<OrderProcessingResult> RunAsync(WorkflowActivityContext context, (OrderRequest Order, PriceCalculation Price) input)
    {
        _logger.LogInformation("Processing accepted offer for order {OrderId}", input.Order.OrderId);

        var result = new OrderProcessingResult
        {
            OrderId = input.Order.OrderId,
            Status = "Confirmed",
            PriceCalculation = input.Price,
            Message = "Order has been confirmed and will be processed for shipment",
            ProcessedAt = DateTime.UtcNow
        };

        _logger.LogInformation("Order {OrderId} confirmed successfully", input.Order.OrderId);

        return Task.FromResult(result);
    }
}
