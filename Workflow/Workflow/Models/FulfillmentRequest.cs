namespace Workflow.Models;

public record FulfillmentRequest(OrderRequest Order, decimal TotalPrice);
