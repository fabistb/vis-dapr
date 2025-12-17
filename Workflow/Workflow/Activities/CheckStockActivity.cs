using Dapr.Workflow;
using Microsoft.Extensions.Logging;
using Workflow.Models;

namespace Workflow.Activities;

public class CheckStockActivity : WorkflowActivity<OrderRequest, StockCheckResult>
{
    private readonly ILogger<CheckStockActivity> _logger;
    private static readonly Dictionary<string, int> _inventoryStock = new()
    {
        { "PROD-001", 100 },
        { "PROD-002", 50 },
        { "PROD-003", 200 },
        { "PROD-004", 0 },
        { "PROD-005", 25 }
    };

    public CheckStockActivity(ILogger<CheckStockActivity> logger)
    {
        _logger = logger;
    }

    public override Task<StockCheckResult> RunAsync(WorkflowActivityContext context, OrderRequest input)
    {
        _logger.LogInformation("Checking stock for order {OrderId}", input.OrderId);

        var result = new StockCheckResult { IsInStock = true };

        foreach (var item in input.Items)
        {
            var availableQuantity = _inventoryStock.GetValueOrDefault(item.ProductId, 0);
            result.AvailableQuantities[item.ProductId] = availableQuantity;

            if (availableQuantity < item.Quantity)
            {
                result.IsInStock = false;
                result.OutOfStockItems.Add(item.ProductName);
                _logger.LogWarning("Product {ProductName} ({ProductId}) is out of stock. Requested: {Requested}, Available: {Available}",
                    item.ProductName, item.ProductId, item.Quantity, availableQuantity);
            }
        }

        _logger.LogInformation("Stock check for order {OrderId} completed. In stock: {IsInStock}", 
            input.OrderId, result.IsInStock);

        return Task.FromResult(result);
    }
}
