using Dapr.Workflow;
using Workflow.Models;
using Workflow.Services;

namespace Workflow.Activities;

internal sealed class CalculatePriceActivity(IInMemoryInventory inventory) : WorkflowActivity<OrderRequest, decimal>
{
    private const decimal DiscountThreshold = 100m;
    private const decimal DiscountRate = 0.10m;

    public override Task<decimal> RunAsync(WorkflowActivityContext context, OrderRequest input)
    {
        var total = input.Items.Sum(i =>
            (inventory.GetUnitPrice(i.ProductId) ?? throw new InvalidOperationException($"Unknown product {i.ProductId}")) * i.Quantity);
        // 10% off orders of 100 or more.
        return Task.FromResult(total >= DiscountThreshold ? Math.Round(total * (1 - DiscountRate), 2) : total);
    }
}
