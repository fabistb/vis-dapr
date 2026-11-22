using Dapr.Workflow;
using Workflow.Models;
using Workflow.Services;

namespace Workflow.Activities;

internal sealed class CheckStockActivity(IInMemoryInventory inventory) : WorkflowActivity<OrderItem, StockCheckResult>
{
    public override Task<StockCheckResult> RunAsync(WorkflowActivityContext context, OrderItem input) =>
        Task.FromResult(new StockCheckResult(input.ProductId, inventory.IsAvailable(input.ProductId, input.Quantity)));
}
