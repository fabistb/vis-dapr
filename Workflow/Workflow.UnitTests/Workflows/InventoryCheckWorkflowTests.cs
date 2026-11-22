using AwesomeAssertions;
using Dapr.Workflow;
using Moq;
using Workflow.Activities;
using Workflow.Models;
using Workflow.Workflows;

namespace Workflow.UnitTests.Workflows;

[TestClass]
public sealed class InventoryCheckWorkflowTests
{
    [TestMethod]
    public async Task CallsCheckStockPerItem_AndAggregatesMissing()
    {
        OrderItem[] items = [new("widget", 1), new("sold-out", 1), new("gizmo", 99)];
        var ctx = new Mock<WorkflowContext>();
        ctx.Setup(c => c.CallActivityAsync<StockCheckResult>(nameof(CheckStockActivity), It.IsAny<OrderItem>(), It.IsAny<WorkflowTaskOptions?>()))
            .ReturnsAsync((string _, object? input, WorkflowTaskOptions? _) =>
            {
                var item = (OrderItem)input!;
                return new StockCheckResult(item.ProductId, item.ProductId == "widget");
            });

        var result = await new InventoryCheckWorkflow().RunAsync(ctx.Object, new OrderRequest("o", "c", [.. items]));

        result.AllAvailable.Should().BeFalse();
        result.MissingProductIds.Should().BeEquivalentTo("sold-out", "gizmo");
        ctx.Verify(c => c.CallActivityAsync<StockCheckResult>(nameof(CheckStockActivity), It.IsAny<OrderItem>(), It.IsAny<WorkflowTaskOptions?>()), Times.Exactly(3));
    }

    [TestMethod]
    public async Task AllInStock_IsAvailable()
    {
        var ctx = new Mock<WorkflowContext>();
        ctx.Setup(c => c.CallActivityAsync<StockCheckResult>(nameof(CheckStockActivity), It.IsAny<OrderItem>(), It.IsAny<WorkflowTaskOptions?>()))
            .ReturnsAsync(new StockCheckResult("widget", true));

        var result = await new InventoryCheckWorkflow().RunAsync(ctx.Object, new OrderRequest("o", "c", [new("widget", 1)]));

        result.Should().BeEquivalentTo(new InventoryCheckResult(true, []));
    }

    [TestMethod]
    public async Task DuplicateProductLines_AreSummedBeforeCheck()
    {
        var ctx = new Mock<WorkflowContext>();
        ctx.Setup(c => c.CallActivityAsync<StockCheckResult>(nameof(CheckStockActivity), It.IsAny<OrderItem>(), It.IsAny<WorkflowTaskOptions?>()))
            .ReturnsAsync(new StockCheckResult("widget", true));

        await new InventoryCheckWorkflow().RunAsync(ctx.Object, new OrderRequest("o", "c", [new("widget", 2), new("widget", 3)]));

        ctx.Verify(c => c.CallActivityAsync<StockCheckResult>(nameof(CheckStockActivity), new OrderItem("widget", 5), It.IsAny<WorkflowTaskOptions?>()), Times.Once());
        ctx.Verify(c => c.CallActivityAsync<StockCheckResult>(nameof(CheckStockActivity), It.IsAny<OrderItem>(), It.IsAny<WorkflowTaskOptions?>()), Times.Once());
    }
}
