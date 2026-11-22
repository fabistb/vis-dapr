using Dapr.Workflow;
using Workflow.Models;
using Workflow.Services;

namespace Workflow.Activities;

internal sealed class ReleaseStockActivity(IInMemoryInventory inventory) : WorkflowActivity<OrderRequest, bool>
{
    public override Task<bool> RunAsync(WorkflowActivityContext context, OrderRequest input)
    {
        inventory.Release(input.Items);
        return Task.FromResult(true);
    }
}
