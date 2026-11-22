using AutoFixture;
using AwesomeAssertions;
using Dapr.Workflow;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Workflow.Activities;
using Workflow.Models;
using Workflow.Workflows;

[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]

namespace Workflow.UnitTests.Workflows;

[TestClass]
public sealed class OrderProcessingWorkflowTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<WorkflowContext> _ctx = new();
    private readonly OrderRequest _order;
    private readonly string _id;

    public OrderProcessingWorkflowTests()
    {
        _id = _fixture.Create<string>();
        _order = new(_fixture.Create<string>(), _fixture.Create<string>(), [new("widget", 1)]);
        _ctx.SetupGet(c => c.InstanceId).Returns(_id);
        _ctx.Setup(c => c.CreateReplaySafeLogger<OrderProcessingWorkflow>()).Returns(NullLogger<OrderProcessingWorkflow>.Instance);

        Activity<ValidationResult>(nameof(ValidateOrderActivity), new(true, null));
        _ctx.Setup(c => c.CallChildWorkflowAsync<InventoryCheckResult>(nameof(InventoryCheckWorkflow), _order, It.IsAny<ChildWorkflowTaskOptions?>()))
            .ReturnsAsync(new InventoryCheckResult(true, []));
        _ctx.Setup(c => c.CallActivityAsync<decimal>(nameof(CalculatePriceActivity), _order, It.IsAny<WorkflowTaskOptions?>()))
            .ReturnsAsync(42m);
        Activity<bool>(nameof(SendOfferActivity), true);
        Activity<bool>(nameof(NotifyDeclinedActivity), true);
    }

    [TestMethod]
    public async Task InvalidOrder_IsRejected()
    {
        Activity(nameof(ValidateOrderActivity), new ValidationResult(false, "bad"));

        var result = await Run();

        result.Should().Be(new OrderResult(_order.OrderId, OrderStatus.Rejected, null, "bad"));
    }

    [TestMethod]
    public async Task MissingStock_IsOutOfStock()
    {
        _ctx.Setup(c => c.CallChildWorkflowAsync<InventoryCheckResult>(nameof(InventoryCheckWorkflow), _order, It.IsAny<ChildWorkflowTaskOptions?>()))
            .ReturnsAsync(new InventoryCheckResult(false, ["sold-out"]));

        var result = await Run();

        result.Status.Should().Be(OrderStatus.OutOfStock);
        result.Message.Should().Contain("sold-out");
    }

    [TestMethod]
    public async Task DeclinedOffer_NotifiesAndIsDeclined()
    {
        OfferAnswer(false);

        var result = await Run();

        result.Status.Should().Be(OrderStatus.Declined);
        _ctx.Verify(c => c.CallActivityAsync<bool>(nameof(NotifyDeclinedActivity), _order.OrderId, It.IsAny<WorkflowTaskOptions?>()), Times.Once);
    }

    [TestMethod]
    public async Task NoAnswer_IsExpired()
    {
        _ctx.Setup(c => c.WaitForExternalEventAsync<OfferResponse>(OrderProcessingWorkflow.OfferResponseEvent, It.IsAny<TimeSpan>()))
            .ThrowsAsync(new TaskCanceledException());

        var result = await Run();

        result.Should().Be(new OrderResult(_order.OrderId, OrderStatus.Expired, 42m, "Offer expired"));
        _ctx.Verify(c => c.SetCustomStatus(null), Times.Once);
    }

    [TestMethod]
    public async Task AcceptedOffer_FulfillmentSucceeds_IsCompleted()
    {
        OfferAnswer(true);
        Fulfillment(new FulfillmentResult(OrderStatus.Completed, "Order shipped"));

        var result = await Run();

        result.Should().Be(new OrderResult(_order.OrderId, OrderStatus.Completed, 42m, "Order shipped"));
        _ctx.Verify(c => c.CallChildWorkflowAsync<InventoryCheckResult>(
            nameof(InventoryCheckWorkflow), _order, It.Is<ChildWorkflowTaskOptions?>(o => o!.InstanceId == $"{_id}-inventory")), Times.Once);
        _ctx.Verify(c => c.CallChildWorkflowAsync<FulfillmentResult>(
            nameof(FulfillmentWorkflow), It.IsAny<FulfillmentRequest>(), It.Is<ChildWorkflowTaskOptions?>(o => o!.InstanceId == $"{_id}-fulfillment")), Times.Once);
    }

    [TestMethod]
    public async Task AcceptedOffer_FulfillmentFails_IsPaymentFailed()
    {
        OfferAnswer(true);
        Fulfillment(new FulfillmentResult(OrderStatus.PaymentFailed, "Payment failed: nope"));

        var result = await Run();

        result.Status.Should().Be(OrderStatus.PaymentFailed);
    }

    private void Activity<T>(string name, T result) =>
        _ctx.Setup(c => c.CallActivityAsync<T>(name, It.IsAny<object?>(), It.IsAny<WorkflowTaskOptions?>())).ReturnsAsync(result);

    private void OfferAnswer(bool accepted) =>
        _ctx.Setup(c => c.WaitForExternalEventAsync<OfferResponse>(OrderProcessingWorkflow.OfferResponseEvent, It.IsAny<TimeSpan>()))
            .ReturnsAsync(new OfferResponse(accepted));

    private void Fulfillment(FulfillmentResult result) =>
        _ctx.Setup(c => c.CallChildWorkflowAsync<FulfillmentResult>(nameof(FulfillmentWorkflow), It.IsAny<FulfillmentRequest>(), It.IsAny<ChildWorkflowTaskOptions?>()))
            .ReturnsAsync(result);

    private Task<OrderResult> Run() => new OrderProcessingWorkflow().RunAsync(_ctx.Object, _order);
}
