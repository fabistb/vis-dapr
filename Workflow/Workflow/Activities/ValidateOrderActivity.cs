using Dapr.Workflow;
using Microsoft.Extensions.Logging;
using Workflow.Models;

namespace Workflow.Activities;

public class ValidateOrderActivity : WorkflowActivity<OrderRequest, OrderValidationResult>
{
    private readonly ILogger<ValidateOrderActivity> _logger;

    public ValidateOrderActivity(ILogger<ValidateOrderActivity> logger)
    {
        _logger = logger;
    }

    public override Task<OrderValidationResult> RunAsync(WorkflowActivityContext context, OrderRequest input)
    {
        _logger.LogInformation("Validating order {OrderId}", input.OrderId);

        var result = new OrderValidationResult { IsValid = true };

        // Validate order has items
        if (input.Items == null || input.Items.Count == 0)
        {
            result.IsValid = false;
            result.ValidationErrors.Add("Order must contain at least one item");
        }

        // Validate customer ID
        if (string.IsNullOrWhiteSpace(input.CustomerId))
        {
            result.IsValid = false;
            result.ValidationErrors.Add("Customer ID is required");
        }

        // Validate delivery address
        if (string.IsNullOrWhiteSpace(input.DeliveryAddress))
        {
            result.IsValid = false;
            result.ValidationErrors.Add("Delivery address is required");
        }

        // Validate items
        foreach (var item in input.Items ?? new List<OrderItem>())
        {
            if (item.Quantity <= 0)
            {
                result.IsValid = false;
                result.ValidationErrors.Add($"Product {item.ProductName} has invalid quantity");
            }

            if (item.UnitPrice < 0)
            {
                result.IsValid = false;
                result.ValidationErrors.Add($"Product {item.ProductName} has invalid price");
            }
        }

        _logger.LogInformation("Order {OrderId} validation result: {IsValid}", input.OrderId, result.IsValid);

        return Task.FromResult(result);
    }
}
