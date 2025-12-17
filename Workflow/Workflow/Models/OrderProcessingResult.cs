namespace Workflow.Models;

public class OrderProcessingResult
{
    public string OrderId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public PriceCalculation? PriceCalculation { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }
}
