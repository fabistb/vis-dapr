using AutoFixture;
using Dapr.Workflow;
using Moq;
using Workflow.Activities;
using Workflow.Models;
using Workflow.Services;

namespace Workflow.UnitTests.Activities;

[TestClass]
public sealed class ReleaseStockActivityTests
{
    private readonly Fixture _fixture = new();
    private readonly WorkflowActivityContext _ctx = new Mock<WorkflowActivityContext>().Object;

    [TestMethod]
    public async Task ReleaseStock_ReleasesOrderItems()
    {
        var order = _fixture.Create<OrderRequest>();
        var inventory = new Mock<IInMemoryInventory>();

        await new ReleaseStockActivity(inventory.Object).RunAsync(_ctx, order);

        inventory.Verify(i => i.Release(order.Items), Times.Once);
    }
}
