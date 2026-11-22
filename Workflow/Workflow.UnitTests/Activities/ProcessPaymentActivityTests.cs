using AutoFixture;
using AwesomeAssertions;
using Dapr.Workflow;
using Moq;
using Workflow.Activities;
using Workflow.Models;

namespace Workflow.UnitTests.Activities;

[TestClass]
public sealed class ProcessPaymentActivityTests
{
    private readonly Fixture _fixture = new();
    private readonly WorkflowActivityContext _ctx = new Mock<WorkflowActivityContext>().Object;

    [TestMethod]
    public async Task ProcessPayment_NormalCustomer_Succeeds()
    {
        var request = new PaymentRequest(_fixture.Create<string>(), _fixture.Create<string>(), 50m);

        var result = await new ProcessPaymentActivity().RunAsync(_ctx, request);

        result.Success.Should().BeTrue();
    }

    [TestMethod]
    public async Task ProcessPayment_BlockedCustomer_Fails()
    {
        var request = new PaymentRequest(_fixture.Create<string>(), ProcessPaymentActivity.BlockedCustomer, 50m);

        var result = await new ProcessPaymentActivity().RunAsync(_ctx, request);

        result.Should().Be(new PaymentResult(false, "Customer is blocked"));
    }

    [TestMethod]
    public async Task ProcessPayment_AboveLimit_Fails()
    {
        var request = new PaymentRequest(_fixture.Create<string>(), _fixture.Create<string>(), ProcessPaymentActivity.PaymentLimit + 0.01m);

        var result = await new ProcessPaymentActivity().RunAsync(_ctx, request);

        result.Success.Should().BeFalse();
        result.Reason.Should().Contain("limit");
    }

    [TestMethod]
    public async Task ProcessPayment_AtLimit_Succeeds()
    {
        var request = new PaymentRequest(_fixture.Create<string>(), _fixture.Create<string>(), ProcessPaymentActivity.PaymentLimit);

        var result = await new ProcessPaymentActivity().RunAsync(_ctx, request);

        result.Success.Should().BeTrue();
    }
}
