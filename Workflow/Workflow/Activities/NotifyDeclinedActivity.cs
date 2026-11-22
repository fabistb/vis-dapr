using Dapr.Workflow;

namespace Workflow.Activities;

internal sealed partial class NotifyDeclinedActivity(ILogger<NotifyDeclinedActivity> logger) : WorkflowActivity<string, bool>
{
    public override Task<bool> RunAsync(WorkflowActivityContext context, string input)
    {
        LogDeclined(input);
        return Task.FromResult(true);
    }

    [LoggerMessage(LogLevel.Information, "Order {OrderId} was declined by the customer")]
    private partial void LogDeclined(string orderId);
}
