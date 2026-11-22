namespace Workflow.Models;

public record PaymentRequest(string OrderId, string CustomerId, decimal Amount);
