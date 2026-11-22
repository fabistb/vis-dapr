using AutoFixture;
using AwesomeAssertions;
using Dapr.Workflow;
using Moq;
using Workflow.Activities;
using Workflow.Models;
using Workflow.Services;

namespace Workflow.UnitTests.Activities;

[TestClass]
public sealed class CalculatePriceActivityTests
{
    private readonly Fixture _fixture = new();
    private readonly WorkflowActivityContext _ctx = new Mock<WorkflowActivityContext>().Object;

    [TestMethod]
    public async Task CalculatePrice_BelowThreshold_NoDiscount()
    {
        var inventory = new Mock<IInMemoryInventory>();
        inventory.Setup(i => i.GetUnitPrice("widget")).Returns(10m);

        var total = await new CalculatePriceActivity(inventory.Object).RunAsync(_ctx, Order(new OrderItem("widget", 9)));

        total.Should().Be(90m);
    }

    [TestMethod]
    public async Task CalculatePrice_AtThreshold_Applies10PercentDiscount()
    {
        var inventory = new Mock<IInMemoryInventory>();
        inventory.Setup(i => i.GetUnitPrice("widget")).Returns(10m);

        var total = await new CalculatePriceActivity(inventory.Object).RunAsync(_ctx, Order(new OrderItem("widget", 10)));

        total.Should().Be(90m);
    }

    [TestMethod]
    public async Task CalculatePrice_UnknownProduct_Throws()
    {
        var inventory = new Mock<IInMemoryInventory>();

        var act = () => new CalculatePriceActivity(inventory.Object).RunAsync(_ctx, Order(new OrderItem("nope", 1)));

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    private OrderRequest Order(params OrderItem[] items) =>
        new(_fixture.Create<string>(), _fixture.Create<string>(), [.. items]);
}
