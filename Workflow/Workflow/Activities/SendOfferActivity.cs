using Dapr.Workflow;
using Workflow.Models;

namespace Workflow.Activities;

internal sealed partial class SendOfferActivity(ILogger<SendOfferActivity> logger) : WorkflowActivity<OfferNotification, bool>
{
    public override Task<bool> RunAsync(WorkflowActivityContext context, OfferNotification input)
    {
        LogOffer(input.OrderId, input.CustomerId, input.TotalPrice);
        return Task.FromResult(true);
    }

    [LoggerMessage(LogLevel.Information, "Offer sent for order {OrderId} to customer {CustomerId}: {TotalPrice}")]
    private partial void LogOffer(string orderId, string customerId, decimal totalPrice);
}
