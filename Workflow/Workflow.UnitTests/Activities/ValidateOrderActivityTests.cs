using AutoFixture;
using AwesomeAssertions;
using Dapr.Workflow;
using Moq;
using Workflow.Activities;
using Workflow.Models;

namespace Workflow.UnitTests.Activities;

[TestClass]
public sealed class ValidateOrderActivityTests
{
    private readonly Fixture _fixture = new();
    private readonly WorkflowActivityContext _ctx = new Mock<WorkflowActivityContext>().Object;

    [TestMethod]
    public async Task ValidateOrder_ValidOrder_IsValid()
    {
        var result = await new ValidateOrderActivity().RunAsync(_ctx, Order(new OrderItem("widget", 1)));

        result.IsValid.Should().BeTrue();
        result.Error.Should().BeNull();
    }

    [TestMethod]
    public async Task ValidateOrder_NoItems_IsInvalid()
    {
        var result = await new ValidateOrderActivity().RunAsync(_ctx, Order());

        result.IsValid.Should().BeFalse();
        result.Error.Should().Contain("no items");
    }

    [TestMethod]
    public async Task ValidateOrder_MissingOrderId_IsInvalid()
    {
        var order = Order(new OrderItem("widget", 1)) with { OrderId = " " };

        var result = await new ValidateOrderActivity().RunAsync(_ctx, order);

        result.Error.Should().Be("OrderId is required");
    }

    [TestMethod]
    public async Task ValidateOrder_MissingCustomerId_IsInvalid()
    {
        var order = Order(new OrderItem("widget", 1)) with { CustomerId = "" };

        var result = await new ValidateOrderActivity().RunAsync(_ctx, order);

        result.Error.Should().Be("CustomerId is required");
    }

    [TestMethod]
    public async Task ValidateOrder_NonPositiveOfferTimeout_IsInvalid()
    {
        var order = Order(new OrderItem("widget", 1)) with { OfferTimeout = TimeSpan.Zero };

        var result = await new ValidateOrderActivity().RunAsync(_ctx, order);

        result.Error.Should().Be("OfferTimeout must be positive");
    }

    [TestMethod]
    [DataRow("widget", 0)]
    [DataRow("widget", -1)]
    [DataRow("", 1)]
    public async Task ValidateOrder_BadItem_IsInvalid(string productId, int quantity)
    {
        var result = await new ValidateOrderActivity().RunAsync(_ctx, Order(new OrderItem(productId, quantity)));

        result.IsValid.Should().BeFalse();
    }

    private OrderRequest Order(params OrderItem[] items) =>
        new(_fixture.Create<string>(), _fixture.Create<string>(), [.. items]);
}
