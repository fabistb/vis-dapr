namespace Workflow.Models;

// OfferTimeout is optional so tests can use short timeouts; null means the 5 minute default.
public record OrderRequest(string OrderId, string CustomerId, List<OrderItem> Items, TimeSpan? OfferTimeout = null);
