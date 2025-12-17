using Dapr.Workflow;
using Microsoft.Extensions.Logging;
using Workflow.Models;

namespace Workflow.Activities;

public class SendOfferActivity : WorkflowActivity<(OrderRequest Order, PriceCalculation Price), string>
{
    private readonly ILogger<SendOfferActivity> _logger;

    public SendOfferActivity(ILogger<SendOfferActivity> logger)
    {
        _logger = logger;
    }

    public override Task<string> RunAsync(WorkflowActivityContext context, (OrderRequest Order, PriceCalculation Price) input)
    {
        _logger.LogInformation("Sending offer for order {OrderId} to customer {CustomerId}",
            input.Order.OrderId, input.Order.CustomerId);

        // Simulate sending offer via email/notification
        var offerMessage = $"Dear Customer {input.Order.CustomerId},\n\n" +
                          $"Your order {input.Order.OrderId} has been processed.\n" +
                          $"Total items: {input.Order.Items.Count}\n" +
                          $"Subtotal: {input.Price.SubTotal:C}\n" +
                          $"Tax: {input.Price.Tax:C}\n" +
                          $"Shipping: {input.Price.ShippingCost:C}\n" +
                          $"Discount: {input.Price.Discount:C}\n" +
                          $"Total Price: {input.Price.TotalPrice:C}\n\n" +
                          $"Please confirm your order.";

        _logger.LogInformation("Offer sent successfully for order {OrderId}", input.Order.OrderId);

        return Task.FromResult($"Offer sent to customer {input.Order.CustomerId}");
    }
}
