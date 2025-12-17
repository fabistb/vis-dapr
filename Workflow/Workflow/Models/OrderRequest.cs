namespace Workflow.Models;

public class OrderRequest
{
    public string OrderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public List<OrderItem> Items { get; set; } = new();
    public string DeliveryAddress { get; set; } = string.Empty;
}

public class OrderItem
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
