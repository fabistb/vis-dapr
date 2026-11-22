using Dapr.Workflow;
using Workflow.Models;
using Workflow.Services;

namespace Workflow.Activities;

internal sealed class ReserveStockActivity(IInMemoryInventory inventory) : WorkflowActivity<OrderRequest, bool>
{
    public override Task<bool> RunAsync(WorkflowActivityContext context, OrderRequest input) =>
        Task.FromResult(inventory.TryReserve(input.OrderId, input.Items));
}
