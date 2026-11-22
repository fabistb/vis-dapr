using AutoFixture;
using AwesomeAssertions;
using Dapr.Workflow;
using Moq;
using Workflow.Activities;
using Workflow.Models;
using Workflow.Services;

namespace Workflow.UnitTests.Activities;

[TestClass]
public sealed class CheckStockActivityTests
{
    private readonly Fixture _fixture = new();
    private readonly WorkflowActivityContext _ctx = new Mock<WorkflowActivityContext>().Object;

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task CheckStock_ReturnsInventoryAvailability(bool available)
    {
        var item = _fixture.Create<OrderItem>();
        var inventory = new Mock<IInMemoryInventory>();
        inventory.Setup(i => i.IsAvailable(item.ProductId, item.Quantity)).Returns(available);

        var result = await new CheckStockActivity(inventory.Object).RunAsync(_ctx, item);

        result.Should().Be(new StockCheckResult(item.ProductId, available));
    }
}
