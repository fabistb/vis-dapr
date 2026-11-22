namespace Workflow.Models;

public record InventoryCheckResult(bool AllAvailable, List<string> MissingProductIds);
