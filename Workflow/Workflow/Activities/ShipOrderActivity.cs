using Dapr.Workflow;

namespace Workflow.Activities;

internal sealed partial class ShipOrderActivity(ILogger<ShipOrderActivity> logger) : WorkflowActivity<string, bool>
{
    public override Task<bool> RunAsync(WorkflowActivityContext context, string input)
    {
        LogShipped(input);
        return Task.FromResult(true);
    }

    [LoggerMessage(LogLevel.Information, "Order {OrderId} shipped")]
    private partial void LogShipped(string orderId);
}
