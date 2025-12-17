using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Workflow.Activities;
using Workflow.Models;

namespace Workflow.UnitTests.Activities;

[TestClass]
public class CalculatePriceActivityTests
{
    private readonly Mock<ILogger<CalculatePriceActivity>> _mockLogger;
    private readonly CalculatePriceActivity _activity;

    public CalculatePriceActivityTests()
    {
        _mockLogger = new Mock<ILogger<CalculatePriceActivity>>();
        _activity = new CalculatePriceActivity(_mockLogger.Object);
    }

    [TestMethod]
    public async Task RunAsync_WithSingleItem_CalculatesCorrectPrice()
    {
        // Arrange
        var order = new OrderRequest
        {
            OrderId = "ORD-001",
            Items = new List<OrderItem>
            {
                new() { ProductId = "PROD-001", ProductName = "Laptop", Quantity = 1, UnitPrice = 100m }
            }
        };

        // Act
        var result = await _activity.RunAsync(null!, order);

        // Assert
        result.SubTotal.Should().Be(100m);
        result.ShippingCost.Should().Be(5m); // 5 per item
        result.Tax.Should().Be(20m); // 20% of subtotal
        result.Discount.Should().Be(0m); // No discount under threshold
        result.TotalPrice.Should().Be(125m); // 100 + 5 + 20
    }

    [TestMethod]
    public async Task RunAsync_WithOrderOverThreshold_AppliesDiscount()
    {
        // Arrange
        var order = new OrderRequest
        {
            OrderId = "ORD-001",
            Items = new List<OrderItem>
            {
                new() { ProductId = "PROD-001", ProductName = "Laptop", Quantity = 2, UnitPrice = 60m }
            }
        };

        // Act
        var result = await _activity.RunAsync(null!, order);

        // Assert
        result.SubTotal.Should().Be(120m);
        result.Discount.Should().Be(12m); // 10% of subtotal
        result.ShippingCost.Should().Be(10m); // 5 per item * 2
        result.Tax.Should().Be(21.6m); // 20% of (120 - 12)
        result.TotalPrice.Should().Be(139.6m); // 120 - 12 + 21.6 + 10
    }

    [TestMethod]
    public async Task RunAsync_WithMultipleItems_CalculatesCorrectly()
    {
        // Arrange
        var order = new OrderRequest
        {
            OrderId = "ORD-001",
            Items = new List<OrderItem>
            {
                new() { ProductId = "PROD-001", ProductName = "Laptop", Quantity = 1, UnitPrice = 50m },
                new() { ProductId = "PROD-002", ProductName = "Mouse", Quantity = 2, UnitPrice = 25m }
            }
        };

        // Act
        var result = await _activity.RunAsync(null!, order);

        // Assert
        result.SubTotal.Should().Be(100m);
        result.ShippingCost.Should().Be(15m); // 5 * 3 items
        result.Tax.Should().Be(20m); // 20% of subtotal
        result.Discount.Should().Be(0m);
        result.TotalPrice.Should().Be(135m);
    }
}
