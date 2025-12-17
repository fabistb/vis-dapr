namespace Workflow.Models;

public class OrderValidationResult
{
    public bool IsValid { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
}
