using AwesomeAssertions;
using Workflow.Models;
using Workflow.Services;

namespace Workflow.UnitTests.Services;

[TestClass]
public sealed class InMemoryInventoryTests
{
    private readonly InMemoryInventory _sut = new();

    [TestMethod]
    public void TryReserve_AllInStock_ReducesStock()
    {
        var reserved = _sut.TryReserve("o", [new("gizmo", 5)]);

        reserved.Should().BeTrue();
        _sut.IsAvailable("gizmo", 1).Should().BeFalse();
    }

    [TestMethod]
    public void TryReserve_OneItemShort_ReservesNothing()
    {
        var reserved = _sut.TryReserve("o", [new("widget", 10), new("sold-out", 1)]);

        reserved.Should().BeFalse();
        _sut.IsAvailable("widget", 100).Should().BeTrue(); // untouched
    }

    [TestMethod]
    public void TryReserve_SameProductTwice_ChecksCombinedQuantity()
    {
        var reserved = _sut.TryReserve("o", [new("gizmo", 3), new("gizmo", 3)]);

        reserved.Should().BeFalse();
        _sut.IsAvailable("gizmo", 5).Should().BeTrue(); // untouched
    }

    [TestMethod]
    public void Release_AfterReserve_RestoresStock()
    {
        OrderItem[] items = [new("gizmo", 5)];
        _sut.TryReserve("o", items);

        _sut.Release("o");

        _sut.IsAvailable("gizmo", 5).Should().BeTrue();
    }

    [TestMethod]
    public void TryReserve_SameOrderTwice_DeductsOnce()
    {
        _sut.TryReserve("o", [new("gizmo", 2)]).Should().BeTrue();
        _sut.TryReserve("o", [new("gizmo", 2)]).Should().BeTrue();

        _sut.IsAvailable("gizmo", 3).Should().BeTrue();
        _sut.IsAvailable("gizmo", 4).Should().BeFalse();
    }

    [TestMethod]
    public void Release_Twice_RestoresOnce()
    {
        _sut.TryReserve("o", [new("gizmo", 2)]);
        _sut.Release("o");
        _sut.Release("o");

        _sut.IsAvailable("gizmo", 5).Should().BeTrue();
        _sut.IsAvailable("gizmo", 6).Should().BeFalse();
    }

    [TestMethod]
    public void Release_UnknownOrder_IsNoOp()
    {
        _sut.Release("unknown");

        _sut.IsAvailable("gizmo", 6).Should().BeFalse();
    }

    [TestMethod]
    public void GetUnitPrice_UnknownProduct_ReturnsNull() =>
        _sut.GetUnitPrice("nope").Should().BeNull();
}
