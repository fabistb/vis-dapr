using System.Reflection;
using Asp.Versioning;
using AwesomeAssertions;
using Dapr.Common.Serialization;
using Dapr.Workflow;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Workflow.Controllers;
using Workflow.Models;
using Workflow.Workflows;

namespace Workflow.UnitTests.Controllers;

[TestClass]
public sealed class OrderControllerTests
{
    private readonly Mock<IDaprWorkflowClient> _client = new();
    private readonly OrderController _controller;

    public OrderControllerTests()
    {
        var http = new DefaultHttpContext();
        http.Features.Set<IApiVersioningFeature>(new ApiVersioningFeature(http) { RequestedApiVersion = new ApiVersion(1, 0) });
        _controller = new OrderController(_client.Object) { ControllerContext = new() { HttpContext = http } };
    }

    [TestMethod]
    public async Task Start_BlankId_ReturnsBadRequest()
    {
        var result = await _controller.Start(new OrderRequest(" ", "c", []));

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [TestMethod]
    [DataRow(OrderProcessingWorkflow.InventorySuffix)]
    [DataRow(OrderProcessingWorkflow.FulfillmentSuffix)]
    public async Task Start_ReservedSuffix_ReturnsBadRequest(string suffix)
    {
        var result = await _controller.Start(new OrderRequest($"o{suffix}", "c", []));

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [TestMethod]
    public async Task Start_ExistingOrder_ReturnsConflict()
    {
        State("o", exists: true);

        var result = await _controller.Start(new OrderRequest("o", "c", []));

        result.Should().BeOfType<ConflictObjectResult>();
    }

    [TestMethod]
    public async Task Start_LosesScheduleRace_ReturnsConflict()
    {
        State("o", exists: false);
        _client.Setup(c => c.ScheduleNewWorkflowAsync(It.IsAny<string>(), "o", It.IsAny<object?>()))
            .ThrowsAsync(new RpcException(new Status(StatusCode.Unknown, "an active workflow with ID 'o' already exists")));

        var result = await _controller.Start(new OrderRequest("o", "c", []));

        result.Should().BeOfType<ConflictObjectResult>();
    }

    [TestMethod]
    public async Task Start_NewOrder_ReturnsAcceptedWithRequestedVersion()
    {
        State("o", exists: false);
        _client.Setup(c => c.ScheduleNewWorkflowAsync(It.IsAny<string>(), "o", It.IsAny<object?>()))
            .ReturnsAsync("o");

        var result = await _controller.Start(new OrderRequest("o", "c", []));

        var accepted = result.Should().BeOfType<AcceptedAtActionResult>().Subject;
        accepted.RouteValues!["v"].Should().Be("1.0");
    }

    [TestMethod]
    public async Task Get_Unknown_ReturnsNotFound()
    {
        State("o", exists: false);

        (await _controller.Get("o")).Should().BeOfType<NotFoundResult>();
    }

    [TestMethod]
    public async Task Get_Completed_ReturnsOutput()
    {
        var output = new OrderResult("o", OrderStatus.Completed, 1m, "done");
        State("o", status: WorkflowRuntimeStatus.Completed, output: output);

        var body = ((await _controller.Get("o")) as OkObjectResult)!.Value!;

        Prop(body, "output").Should().Be(output);
    }

    [TestMethod]
    public async Task Get_Failed_ReturnsFailureDetails()
    {
        State("o", status: WorkflowRuntimeStatus.Failed, failure: new WorkflowTaskFailureDetails("MyError", "boom", null));

        var error = Prop(((await _controller.Get("o")) as OkObjectResult)!.Value!, "error")!;

        Prop(error, "type").Should().Be("MyError");
        Prop(error, "message").Should().Be("boom");
    }

    [TestMethod]
    public async Task Respond_Unknown_ReturnsNotFound()
    {
        State("o", exists: false);

        (await _controller.Respond("o", new OfferResponse(true))).Should().BeOfType<NotFoundResult>();
    }

    [TestMethod]
    public async Task Respond_Completed_ReturnsConflict()
    {
        State("o", status: WorkflowRuntimeStatus.Completed);

        (await _controller.Respond("o", new OfferResponse(true))).Should().BeOfType<ConflictObjectResult>();
    }

    [TestMethod]
    public async Task Respond_NotAwaitingOffer_ReturnsConflict()
    {
        State("o", customStatus: "Validating");

        (await _controller.Respond("o", new OfferResponse(true))).Should().BeOfType<ConflictObjectResult>();
        _client.Verify(c => c.RaiseEventAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object?>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [TestMethod]
    public async Task Respond_AwaitingOffer_RaisesEventAndReturnsAccepted()
    {
        State("o", customStatus: OrderProcessingWorkflow.AwaitingOfferStatus);

        (await _controller.Respond("o", new OfferResponse(true))).Should().BeOfType<AcceptedResult>();
        _client.Verify(c => c.RaiseEventAsync("o", OrderProcessingWorkflow.OfferResponseEvent, It.IsAny<object?>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    private void State(string id, bool exists = true, WorkflowRuntimeStatus status = WorkflowRuntimeStatus.Running,
        string? customStatus = null, OrderResult? output = null, WorkflowTaskFailureDetails? failure = null)
    {
        var serializer = new Mock<IDaprSerializer>();
        serializer.Setup(s => s.Deserialize<string>(It.IsAny<string>())).Returns(customStatus);
        serializer.Setup(s => s.Deserialize<OrderResult>(It.IsAny<string>())).Returns(output);
        _client.Setup(c => c.GetWorkflowStateAsync(id, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(NewState(exists ? NewMetadata(id, status, serializer.Object, failure) : null));
    }

    // WorkflowState and WorkflowMetadata have only internal constructors, so build them reflectively.
    private static object NewMetadata(string id, WorkflowRuntimeStatus status, IDaprSerializer serializer, WorkflowTaskFailureDetails? failure)
    {
        var type = typeof(WorkflowState).Assembly.GetType("Dapr.Workflow.Client.WorkflowMetadata")!;
        var metadata = Activator.CreateInstance(type, id, nameof(OrderProcessingWorkflow), status, DateTime.UtcNow, DateTime.UtcNow, serializer)!;
        type.GetProperty("SerializedCustomStatus")!.SetValue(metadata, "x");
        type.GetProperty("SerializedOutput")!.SetValue(metadata, "x");
        type.GetProperty("FailureDetails")!.SetValue(metadata, failure);
        return metadata;
    }

    private static WorkflowState NewState(object? metadata) =>
        (WorkflowState)Activator.CreateInstance(typeof(WorkflowState), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, [metadata], null)!;

    private static object? Prop(object o, string name) => o.GetType().GetProperty(name)!.GetValue(o);
}
