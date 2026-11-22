using Dapr.Workflow;
using Workflow.Activities;
using Workflow.Models;

namespace Workflow.Workflows;

internal sealed partial class OrderProcessingWorkflow : Workflow<OrderRequest, OrderResult>
{
    public const string OfferResponseEvent = "OfferResponse";
    public const string AwaitingOfferStatus = "AwaitingOfferResponse";
    // Child workflow ids are the order id plus these suffixes; the controller rejects order ids ending in them.
    public const string InventorySuffix = "-inventory";
    public const string FulfillmentSuffix = "-fulfillment";
    public static readonly TimeSpan DefaultOfferTimeout = TimeSpan.FromMinutes(5);

    private static readonly WorkflowTaskOptions PricingOptions =
        new(new WorkflowRetryPolicy(maxNumberOfAttempts: 3, firstRetryInterval: TimeSpan.FromSeconds(1), backoffCoefficient: 2.0));

    public override async Task<OrderResult> RunAsync(WorkflowContext context, OrderRequest input)
    {
        var logger = context.CreateReplaySafeLogger<OrderProcessingWorkflow>();
        LogStarted(logger, input.OrderId);

        var validation = await context.CallActivityAsync<ValidationResult>(nameof(ValidateOrderActivity), input);
        if (!validation.IsValid)
            return new OrderResult(input.OrderId, OrderStatus.Rejected, null, validation.Error ?? "Invalid order");

        var inventory = await context.CallChildWorkflowAsync<InventoryCheckResult>(
            nameof(InventoryCheckWorkflow), input, new ChildWorkflowTaskOptions { InstanceId = context.InstanceId + InventorySuffix });
        if (!inventory.AllAvailable)
            return new OrderResult(input.OrderId, OrderStatus.OutOfStock, null,
                $"Out of stock: {string.Join(", ", inventory.MissingProductIds)}");

        var total = await context.CallActivityAsync<decimal>(nameof(CalculatePriceActivity), input, PricingOptions);

        await context.CallActivityAsync<bool>(nameof(SendOfferActivity), new OfferNotification(input.OrderId, input.CustomerId, total));
        context.SetCustomStatus(AwaitingOfferStatus);

        OfferResponse response;
        try
        {
            response = await context.WaitForExternalEventAsync<OfferResponse>(OfferResponseEvent, input.OfferTimeout ?? DefaultOfferTimeout);
        }
        catch (TaskCanceledException)
        {
            return new OrderResult(input.OrderId, OrderStatus.Expired, total, "Offer expired");
        }
        finally
        {
            context.SetCustomStatus(null);
        }

        if (!response.Accepted)
        {
            await context.CallActivityAsync<bool>(nameof(NotifyDeclinedActivity), input.OrderId);
            return new OrderResult(input.OrderId, OrderStatus.Declined, total, "Offer declined");
        }

        var fulfillment = await context.CallChildWorkflowAsync<FulfillmentResult>(
            nameof(FulfillmentWorkflow), new FulfillmentRequest(input, total),
            new ChildWorkflowTaskOptions { InstanceId = context.InstanceId + FulfillmentSuffix });

        return new OrderResult(input.OrderId, fulfillment.Status, total, fulfillment.Message);
    }

    [LoggerMessage(LogLevel.Information, "Order workflow started for {OrderId}")]
    private static partial void LogStarted(ILogger logger, string orderId);
}
