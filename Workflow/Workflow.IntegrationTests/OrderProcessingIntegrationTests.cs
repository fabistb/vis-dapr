using Microsoft.Extensions.Logging;
using Testcontainers.Redis;
using Workflow.Models;
using Workflow.Workflows;

namespace Workflow.IntegrationTests;

[TestClass]
public class OrderProcessingIntegrationTests
{
    private static RedisContainer? _redisContainer;

    [ClassInitialize]
    public static async Task ClassInitialize(TestContext context)
    {
        _redisContainer = new RedisBuilder()
            .WithImage("redis:7-alpine")
            .Build();

        await _redisContainer.StartAsync();
    }

    [ClassCleanup]
    public static async Task ClassCleanup()
    {
        if (_redisContainer != null)
        {
            await _redisContainer.DisposeAsync();
        }
    }

    [TestMethod]
    public async Task ProcessOrderAsync_WithValidOrder_ReturnsSuccess()
    {
        // Arrange
        var loggerFactory = new LoggerFactory();
        var logger = loggerFactory.CreateLogger<OrderProcessor>();
        var processor = new OrderProcessor(logger, loggerFactory);

        var order = new OrderRequest
        {
            OrderId = "INT-ORD-001",
            CustomerId = "CUST-123",
            DeliveryAddress = "456 Test Ave",
            Items = new List<OrderItem>
            {
                new() { ProductId = "PROD-001", ProductName = "Laptop", Quantity = 1, UnitPrice = 999.99m }
            }
        };

        // Act
        var result = await processor.ProcessOrderAsync(order);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("OfferSent", result.Status);
        Assert.AreEqual(order.OrderId, result.OrderId);
        Assert.IsNotNull(result.PriceCalculation);
        Assert.IsTrue(result.PriceCalculation.TotalPrice > 0);
    }

    [TestMethod]
    public async Task ProcessOrderAsync_WithInvalidOrder_ReturnsValidationFailed()
    {
        // Arrange
        var loggerFactory = new LoggerFactory();
        var logger = loggerFactory.CreateLogger<OrderProcessor>();
        var processor = new OrderProcessor(logger, loggerFactory);

        var order = new OrderRequest
        {
            OrderId = "INT-ORD-002",
            CustomerId = "", // Missing customer ID
            DeliveryAddress = "456 Test Ave",
            Items = new List<OrderItem>
            {
                new() { ProductId = "PROD-001", ProductName = "Laptop", Quantity = 1, UnitPrice = 999.99m }
            }
        };

        // Act
        var result = await processor.ProcessOrderAsync(order);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("ValidationFailed", result.Status);
        Assert.IsTrue(result.Message.Contains("Customer ID is required"));
    }

    [TestMethod]
    public async Task ProcessOrderAsync_WithOutOfStockItem_ReturnsOutOfStock()
    {
        // Arrange
        var loggerFactory = new LoggerFactory();
        var logger = loggerFactory.CreateLogger<OrderProcessor>();
        var processor = new OrderProcessor(logger, loggerFactory);

        var order = new OrderRequest
        {
            OrderId = "INT-ORD-003",
            CustomerId = "CUST-123",
            DeliveryAddress = "456 Test Ave",
            Items = new List<OrderItem>
            {
                new() { ProductId = "PROD-004", ProductName = "OutOfStock Item", Quantity = 1, UnitPrice = 99.99m }
            }
        };

        // Act
        var result = await processor.ProcessOrderAsync(order);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("OutOfStock", result.Status);
        Assert.IsTrue(result.Message.Contains("Out of stock"));
    }

    [TestMethod]
    public async Task ProcessOrderAsync_WithLargeOrder_AppliesDiscount()
    {
        // Arrange
        var loggerFactory = new LoggerFactory();
        var logger = loggerFactory.CreateLogger<OrderProcessor>();
        var processor = new OrderProcessor(logger, loggerFactory);

        var order = new OrderRequest
        {
            OrderId = "INT-ORD-004",
            CustomerId = "CUST-123",
            DeliveryAddress = "456 Test Ave",
            Items = new List<OrderItem>
            {
                new() { ProductId = "PROD-001", ProductName = "Laptop", Quantity = 2, UnitPrice = 60m }
            }
        };

        // Act
        var result = await processor.ProcessOrderAsync(order);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("OfferSent", result.Status);
        Assert.IsNotNull(result.PriceCalculation);
        Assert.IsTrue(result.PriceCalculation.Discount > 0, "Discount should be applied for orders over threshold");
    }
}
