namespace Workflow.Models;

public class OfferResponse
{
    public string OrderId { get; set; } = string.Empty;
    public bool IsAccepted { get; set; }
    public string Reason { get; set; } = string.Empty;
}
