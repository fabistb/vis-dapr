namespace Workflow.Models;

public class PriceCalculation
{
    public decimal SubTotal { get; set; }
    public decimal Tax { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalPrice { get; set; }
}
