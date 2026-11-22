using Dapr.Workflow;
using Workflow.Activities;
using Workflow.Models;

namespace Workflow.Workflows;

internal sealed class FulfillmentWorkflow : Workflow<FulfillmentRequest, FulfillmentResult>
{
    public override async Task<FulfillmentResult> RunAsync(WorkflowContext context, FulfillmentRequest input)
    {
        var order = input.Order;

        if (!await context.CallActivityAsync<bool>(nameof(ReserveStockActivity), order))
            return new FulfillmentResult(OrderStatus.OutOfStock, "Stock could not be reserved");

        PaymentResult payment;
        try
        {
            payment = await context.CallActivityAsync<PaymentResult>(
                nameof(ProcessPaymentActivity), new PaymentRequest(order.OrderId, order.CustomerId, input.TotalPrice));

            if (payment.Success)
                await context.CallActivityAsync<bool>(nameof(ShipOrderActivity), order.OrderId);
        }
        catch (WorkflowTaskFailedException)
        {
            // An activity failed after its retries: release the reservation, then fail the workflow.
            await context.CallActivityAsync<bool>(nameof(ReleaseStockActivity), order);
            throw;
        }

        if (!payment.Success)
        {
            // Compensation outside the try so a failing release is not retried by the catch.
            await context.CallActivityAsync<bool>(nameof(ReleaseStockActivity), order);
            return new FulfillmentResult(OrderStatus.PaymentFailed, $"Payment failed: {payment.Reason}");
        }

        return new FulfillmentResult(OrderStatus.Completed, "Order shipped");
    }
}
