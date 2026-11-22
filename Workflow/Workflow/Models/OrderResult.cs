namespace Workflow.Models;

public record OrderResult(string OrderId, OrderStatus Status, decimal? TotalPrice, string Message);
