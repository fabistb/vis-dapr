using AutoFixture;
using AwesomeAssertions;
using Dapr.Workflow;
using Moq;
using Workflow.Activities;
using Workflow.Models;
using Workflow.Workflows;

namespace Workflow.UnitTests.Workflows;

[TestClass]
public sealed class FulfillmentWorkflowTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<WorkflowContext> _ctx = new();
    private readonly FulfillmentRequest _request;

    public FulfillmentWorkflowTests()
    {
        var order = new OrderRequest(_fixture.Create<string>(), _fixture.Create<string>(), [new("widget", 1)]);
        _request = new FulfillmentRequest(order, 42m);
        Activity(nameof(ReserveStockActivity), true);
        Activity(nameof(ReleaseStockActivity), true);
        Activity(nameof(ShipOrderActivity), true);
    }

    [TestMethod]
    public async Task Success_ShipsOrder()
    {
        Activity(nameof(ProcessPaymentActivity), new PaymentResult(true, null));

        var result = await new FulfillmentWorkflow().RunAsync(_ctx.Object, _request);

        result.Status.Should().Be(OrderStatus.Completed);
        Called(nameof(ShipOrderActivity), Times.Once());
        Called(nameof(ReleaseStockActivity), Times.Never());
    }

    [TestMethod]
    public async Task PaymentFails_ReleasesStock()
    {
        Activity(nameof(ProcessPaymentActivity), new PaymentResult(false, "no money"));

        var result = await new FulfillmentWorkflow().RunAsync(_ctx.Object, _request);

        result.Should().Be(new FulfillmentResult(OrderStatus.PaymentFailed, "Payment failed: no money"));
        Called(nameof(ReleaseStockActivity), Times.Once());
        Called(nameof(ShipOrderActivity), Times.Never());
    }

    [TestMethod]
    public async Task ShipFails_ReleasesStockAndRethrows()
    {
        Activity(nameof(ProcessPaymentActivity), new PaymentResult(true, null));
        _ctx.Setup(c => c.CallActivityAsync<bool>(nameof(ShipOrderActivity), It.IsAny<string>(), It.IsAny<WorkflowTaskOptions?>()))
            .ThrowsAsync(new WorkflowTaskFailedException("failed", new WorkflowTaskFailureDetails("Error", "boom", null)));

        var act = () => new FulfillmentWorkflow().RunAsync(_ctx.Object, _request);

        await act.Should().ThrowAsync<WorkflowTaskFailedException>();
        Called(nameof(ReleaseStockActivity), Times.Once());
    }

    [TestMethod]
    public async Task ReserveFails_StopsEarly()
    {
        Activity(nameof(ReserveStockActivity), false);

        var result = await new FulfillmentWorkflow().RunAsync(_ctx.Object, _request);

        result.Status.Should().Be(OrderStatus.OutOfStock);
        Called(nameof(ShipOrderActivity), Times.Never());
    }

    private void Activity<T>(string name, T result) =>
        _ctx.Setup(c => c.CallActivityAsync<T>(name, It.IsAny<object?>(), It.IsAny<WorkflowTaskOptions?>())).ReturnsAsync(result);

    private void Called(string name, Times times) =>
        _ctx.Verify(c => c.CallActivityAsync<bool>(name, It.IsAny<object?>(), It.IsAny<WorkflowTaskOptions?>()), times);
}
