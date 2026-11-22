using Dapr.Workflow;
using Workflow.Models;

namespace Workflow.Activities;

internal sealed class ProcessPaymentActivity : WorkflowActivity<PaymentRequest, PaymentResult>
{
    internal const decimal PaymentLimit = 10_000m;
    internal const string BlockedCustomer = "blocked";

    // Deterministic in-memory rule instead of a real payment provider.
    public override Task<PaymentResult> RunAsync(WorkflowActivityContext context, PaymentRequest input) =>
        Task.FromResult(
            input.CustomerId == BlockedCustomer ? new PaymentResult(false, "Customer is blocked") :
            input.Amount > PaymentLimit ? new PaymentResult(false, $"Amount exceeds limit of {PaymentLimit}") :
            new PaymentResult(true, null));
}
