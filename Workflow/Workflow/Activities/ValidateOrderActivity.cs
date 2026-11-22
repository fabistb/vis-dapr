using Dapr.Workflow;
using Workflow.Models;

namespace Workflow.Activities;

internal sealed class ValidateOrderActivity : WorkflowActivity<OrderRequest, ValidationResult>
{
    public override Task<ValidationResult> RunAsync(WorkflowActivityContext context, OrderRequest input)
    {
        string? error =
            string.IsNullOrWhiteSpace(input.OrderId) ? "OrderId is required" :
            string.IsNullOrWhiteSpace(input.CustomerId) ? "CustomerId is required" :
            input.Items is not { Count: > 0 } ? "Order has no items" :
            input.Items.Any(i => i is null || string.IsNullOrWhiteSpace(i.ProductId) || i.Quantity <= 0) ? "Items need a product id and quantity > 0" :
            input.Items.GroupBy(i => i.ProductId).Any(g => g.Sum(i => (long)i.Quantity) > int.MaxValue) ? "Quantity too large" :
            input.OfferTimeout <= TimeSpan.Zero ? "OfferTimeout must be positive" :
            null;
        return Task.FromResult(new ValidationResult(error is null, error));
    }
}
