using AutoFixture;
using AwesomeAssertions;
using Dapr.Workflow;
using Moq;
using Workflow.Activities;
using Workflow.Models;
using Workflow.Services;

namespace Workflow.UnitTests.Activities;

[TestClass]
public sealed class ReserveStockActivityTests
{
    private readonly Fixture _fixture = new();
    private readonly WorkflowActivityContext _ctx = new Mock<WorkflowActivityContext>().Object;

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task ReserveStock_ReturnsInventoryResult(bool reserved)
    {
        var order = _fixture.Create<OrderRequest>();
        var inventory = new Mock<IInMemoryInventory>();
        inventory.Setup(i => i.TryReserve(order.Items)).Returns(reserved);

        var result = await new ReserveStockActivity(inventory.Object).RunAsync(_ctx, order);

        result.Should().Be(reserved);
    }
}
