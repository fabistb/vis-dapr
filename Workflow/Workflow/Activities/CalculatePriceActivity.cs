using Dapr.Workflow;
using Microsoft.Extensions.Logging;
using Workflow.Models;

namespace Workflow.Activities;

public class CalculatePriceActivity : WorkflowActivity<OrderRequest, PriceCalculation>
{
    private readonly ILogger<CalculatePriceActivity> _logger;
    private const decimal TaxRate = 0.20m; // 20% tax
    private const decimal ShippingCostPerItem = 5.00m;
    private const decimal DiscountThreshold = 100.00m;
    private const decimal DiscountRate = 0.10m; // 10% discount

    public CalculatePriceActivity(ILogger<CalculatePriceActivity> logger)
    {
        _logger = logger;
    }

    public override Task<PriceCalculation> RunAsync(WorkflowActivityContext context, OrderRequest input)
    {
        _logger.LogInformation("Calculating price for order {OrderId}", input.OrderId);

        var calculation = new PriceCalculation();

        // Calculate subtotal
        calculation.SubTotal = input.Items.Sum(item => item.UnitPrice * item.Quantity);

        // Calculate discount if order exceeds threshold
        if (calculation.SubTotal > DiscountThreshold)
        {
            calculation.Discount = calculation.SubTotal * DiscountRate;
            _logger.LogInformation("Applied discount of {Discount:C} for order {OrderId}", 
                calculation.Discount, input.OrderId);
        }

        // Calculate shipping cost based on total items
        var totalItems = input.Items.Sum(item => item.Quantity);
        calculation.ShippingCost = ShippingCostPerItem * totalItems;

        // Calculate tax on subtotal minus discount
        var taxableAmount = calculation.SubTotal - calculation.Discount;
        calculation.Tax = taxableAmount * TaxRate;

        // Calculate total
        calculation.TotalPrice = calculation.SubTotal - calculation.Discount + calculation.Tax + calculation.ShippingCost;

        _logger.LogInformation("Price calculation for order {OrderId}: SubTotal={SubTotal:C}, Tax={Tax:C}, Shipping={Shipping:C}, Discount={Discount:C}, Total={Total:C}",
            input.OrderId, calculation.SubTotal, calculation.Tax, calculation.ShippingCost, calculation.Discount, calculation.TotalPrice);

        return Task.FromResult(calculation);
    }
}
