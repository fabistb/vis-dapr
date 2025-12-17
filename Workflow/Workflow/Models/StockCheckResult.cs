namespace Workflow.Models;

public class StockCheckResult
{
    public bool IsInStock { get; set; }
    public Dictionary<string, int> AvailableQuantities { get; set; } = new();
    public List<string> OutOfStockItems { get; set; } = new();
}
