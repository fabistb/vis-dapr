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
        var reserved = _sut.TryReserve([new("gizmo", 5)]);

        reserved.Should().BeTrue();
        _sut.IsAvailable("gizmo", 1).Should().BeFalse();
    }

    [TestMethod]
    public void TryReserve_OneItemShort_ReservesNothing()
    {
        var reserved = _sut.TryReserve([new("widget", 10), new("sold-out", 1)]);

        reserved.Should().BeFalse();
        _sut.IsAvailable("widget", 100).Should().BeTrue(); // untouched
    }

    [TestMethod]
    public void TryReserve_SameProductTwice_ChecksCombinedQuantity()
    {
        var reserved = _sut.TryReserve([new("gizmo", 3), new("gizmo", 3)]);

        reserved.Should().BeFalse();
        _sut.IsAvailable("gizmo", 5).Should().BeTrue(); // untouched
    }

    [TestMethod]
    public void Release_AfterReserve_RestoresStock()
    {
        OrderItem[] items = [new("gizmo", 5)];
        _sut.TryReserve(items);

        _sut.Release(items);

        _sut.IsAvailable("gizmo", 5).Should().BeTrue();
    }

    [TestMethod]
    public void GetUnitPrice_UnknownProduct_ReturnsNull() =>
        _sut.GetUnitPrice("nope").Should().BeNull();
}
