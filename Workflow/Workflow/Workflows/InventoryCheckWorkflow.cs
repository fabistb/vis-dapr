using Dapr.Workflow;
using Workflow.Activities;
using Workflow.Models;

namespace Workflow.Workflows;

internal sealed class InventoryCheckWorkflow : Workflow<OrderRequest, InventoryCheckResult>
{
    public override async Task<InventoryCheckResult> RunAsync(WorkflowContext context, OrderRequest input)
    {
        // Sum duplicate lines per product, like ReserveStock does, so the check matches the reservation.
        // Fan-out: one activity per product; fan-in: Task.WhenAll.
        var checks = input.Items
            .GroupBy(i => i.ProductId)
            .Select(g => new OrderItem(g.Key, g.Sum(i => i.Quantity)))
            .Select(i => context.CallActivityAsync<StockCheckResult>(nameof(CheckStockActivity), i))
            .ToList();
        var results = await Task.WhenAll(checks);

        var missing = results.Where(r => !r.Available).Select(r => r.ProductId).Distinct().ToList();
        return new InventoryCheckResult(missing.Count == 0, missing);
    }
}
