using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Workflow.Activities;
using Workflow.Models;

namespace Workflow.UnitTests.Activities;

[TestClass]
public class ValidateOrderActivityTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ILogger<ValidateOrderActivity>> _mockLogger;
    private readonly ValidateOrderActivity _activity;

    public ValidateOrderActivityTests()
    {
        _fixture = new Fixture();
        _mockLogger = new Mock<ILogger<ValidateOrderActivity>>();
        _activity = new ValidateOrderActivity(_mockLogger.Object);
    }

    [TestMethod]
    public async Task RunAsync_WithValidOrder_ReturnsValidResult()
    {
        // Arrange
        var order = new OrderRequest
        {
            OrderId = "ORD-001",
            CustomerId = "CUST-123",
            DeliveryAddress = "123 Main St",
            Items = new List<OrderItem>
            {
                new() { ProductId = "PROD-001", ProductName = "Laptop", Quantity = 1, UnitPrice = 999.99m }
            }
        };

        // Act
        var result = await _activity.RunAsync(null!, order);

        // Assert
        result.IsValid.Should().BeTrue();
        result.ValidationErrors.Should().BeEmpty();
    }

    [TestMethod]
    public async Task RunAsync_WithEmptyItems_ReturnsInvalidResult()
    {
        // Arrange
        var order = new OrderRequest
        {
            OrderId = "ORD-001",
            CustomerId = "CUST-123",
            DeliveryAddress = "123 Main St",
            Items = new List<OrderItem>()
        };

        // Act
        var result = await _activity.RunAsync(null!, order);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ValidationErrors.Should().NotBeEmpty();
        result.ValidationErrors.Should().Contain("Order must contain at least one item");
    }

    [TestMethod]
    public async Task RunAsync_WithMissingCustomerId_ReturnsInvalidResult()
    {
        // Arrange
        var order = new OrderRequest
        {
            OrderId = "ORD-001",
            CustomerId = "",
            DeliveryAddress = "123 Main St",
            Items = new List<OrderItem>
            {
                new() { ProductId = "PROD-001", ProductName = "Laptop", Quantity = 1, UnitPrice = 999.99m }
            }
        };

        // Act
        var result = await _activity.RunAsync(null!, order);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ValidationErrors.Should().Contain("Customer ID is required");
    }

    [TestMethod]
    public async Task RunAsync_WithMissingDeliveryAddress_ReturnsInvalidResult()
    {
        // Arrange
        var order = new OrderRequest
        {
            OrderId = "ORD-001",
            CustomerId = "CUST-123",
            DeliveryAddress = "",
            Items = new List<OrderItem>
            {
                new() { ProductId = "PROD-001", ProductName = "Laptop", Quantity = 1, UnitPrice = 999.99m }
            }
        };

        // Act
        var result = await _activity.RunAsync(null!, order);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ValidationErrors.Should().Contain("Delivery address is required");
    }

    [TestMethod]
    public async Task RunAsync_WithInvalidQuantity_ReturnsInvalidResult()
    {
        // Arrange
        var order = new OrderRequest
        {
            OrderId = "ORD-001",
            CustomerId = "CUST-123",
            DeliveryAddress = "123 Main St",
            Items = new List<OrderItem>
            {
                new() { ProductId = "PROD-001", ProductName = "Laptop", Quantity = 0, UnitPrice = 999.99m }
            }
        };

        // Act
        var result = await _activity.RunAsync(null!, order);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ValidationErrors.Should().Contain("Product Laptop has invalid quantity");
    }

    [TestMethod]
    public async Task RunAsync_WithNegativePrice_ReturnsInvalidResult()
    {
        // Arrange
        var order = new OrderRequest
        {
            OrderId = "ORD-001",
            CustomerId = "CUST-123",
            DeliveryAddress = "123 Main St",
            Items = new List<OrderItem>
            {
                new() { ProductId = "PROD-001", ProductName = "Laptop", Quantity = 1, UnitPrice = -10m }
            }
        };

        // Act
        var result = await _activity.RunAsync(null!, order);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ValidationErrors.Should().Contain("Product Laptop has invalid price");
    }
}
