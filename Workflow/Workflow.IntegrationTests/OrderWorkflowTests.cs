using Dapr.Testcontainers.Common;
using Dapr.Testcontainers.Common.Testing;
using Dapr.Testcontainers.Harnesses;
using Dapr.Workflow;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Workflow.Infrastructure;
using Workflow.Models;
using Workflow.Workflows;

[assembly: DoNotParallelize]

namespace Workflow.IntegrationTests;

// One daprd sidecar + app for the whole class: starting containers is slow.
[TestClass]
public sealed class OrderWorkflowTests
{
    private static DaprTestEnvironment _environment = null!;
    private static DaprTestApplication _app = null!;
    private static IServiceScope _scope = null!;
    private static IDaprWorkflowClient _client = null!;

    public TestContext TestContext { get; set; } = null!;

    [ClassInitialize]
    public static async Task ClassInitialize(TestContext context)
    {
        var ct = context.CancellationToken;
        var componentsDir = TestDirectoryManager.CreateTestDirectory("workflow-components");

        _environment = await DaprTestEnvironment.CreateWithPooledNetworkAsync(needsActorState: true, cancellationToken: ct);
        await _environment.StartAsync(ct);

        var harness = new DaprHarnessBuilder(componentsDir).WithEnvironment(_environment).BuildWorkflow();
        _app = await DaprHarnessBuilder.ForHarness(harness)
            .ConfigureServices(builder => builder.Services.AddOrderWorkflows())
            .BuildAndStartAsync();

        _scope = _app.CreateScope();
        _client = _scope.ServiceProvider.GetRequiredService<IDaprWorkflowClient>();
    }

    [ClassCleanup]
    public static async Task ClassCleanup()
    {
        _scope?.Dispose();
        if (_app is not null) await _app.DisposeAsync();
        if (_environment is not null) await _environment.DisposeAsync();
    }

    [TestMethod]
    [DataRow(true, OrderStatus.Completed)]
    [DataRow(false, OrderStatus.Declined)]
    public async Task ValidOrder_FinishesAccordingToOfferResponse(bool accepted, OrderStatus expected)
    {
        var id = $"order-{Guid.NewGuid()}";
        await StartAsync(id, Order(id));
        await WaitForOfferAsync(id);

        await _client.RaiseEventAsync(id, OrderProcessingWorkflow.OfferResponseEvent, new OfferResponse(accepted), TestContext.CancellationToken);

        Assert.AreEqual(expected, (await CompleteAsync(id)).Status);
    }

    [TestMethod]
    public async Task InvalidOrder_IsRejectedWithoutOffer()
    {
        var id = $"order-{Guid.NewGuid()}";
        await StartAsync(id, new OrderRequest(id, "alice", []));

        Assert.AreEqual(OrderStatus.Rejected, (await CompleteAsync(id)).Status);
    }

    [TestMethod]
    public async Task OfferWithoutResponse_Expires()
    {
        var id = $"order-{Guid.NewGuid()}";
        await StartAsync(id, Order(id, timeout: TimeSpan.FromSeconds(3)));

        Assert.AreEqual(OrderStatus.Expired, (await CompleteAsync(id)).Status);
    }

    [TestMethod]
    public async Task SchedulingSameIdTwice_ThrowsRpcExceptionAlreadyExists()
    {
        var id = $"order-{Guid.NewGuid()}";
        await StartAsync(id, Order(id, timeout: TimeSpan.FromSeconds(3)));

        var ex = await Assert.ThrowsAsync<RpcException>(() => StartAsync(id, Order(id)));

        StringAssert.Contains(ex.Status.Detail, "already exists");
    }

    private static OrderRequest Order(string id, string customer = "alice", TimeSpan? timeout = null, string product = "widget") =>
        new(id, customer, [new OrderItem(product, 1)], timeout);

    private Task StartAsync(string id, OrderRequest order) =>
        _client.ScheduleNewWorkflowAsync(nameof(OrderProcessingWorkflow), id, order);

    private async Task WaitForOfferAsync(string id)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.CancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(30));
        while (true)
        {
            var state = await _client.GetWorkflowStateAsync(id, getInputsAndOutputs: true, cts.Token);
            if (state.ReadCustomStatusAs<string>() == OrderProcessingWorkflow.AwaitingOfferStatus) return;
            await Task.Delay(200, cts.Token);
        }
    }

    private async Task<OrderResult> CompleteAsync(string id)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.CancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(60));
        var state = await _client.WaitForWorkflowCompletionAsync(id, true, cts.Token);
        Assert.AreEqual(WorkflowRuntimeStatus.Completed, state.RuntimeStatus);
        return state.ReadOutputAs<OrderResult>()!;
    }
}
