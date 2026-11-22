using System.Collections.Concurrent;
using Workflow.Models;

namespace Workflow.Services;

public sealed class InMemoryInventory : IInMemoryInventory
{
    private readonly ConcurrentDictionary<string, (int Stock, decimal Price)> _products = new(
        new Dictionary<string, (int, decimal)>
        {
            ["widget"] = (100, 9.99m),
            ["gadget"] = (50, 24.50m),
            ["gizmo"] = (5, 199.00m),
            ["sold-out"] = (0, 5.00m),
        });

    // Guards the multi-item reserve/release so an order is all-or-nothing.
    private readonly object _lock = new();

    public bool IsAvailable(string productId, int quantity) =>
        _products.TryGetValue(productId, out var p) && p.Stock >= quantity;

    public decimal? GetUnitPrice(string productId) =>
        _products.TryGetValue(productId, out var p) ? p.Price : null;

    public bool TryReserve(IEnumerable<OrderItem> items)
    {
        // Sum per product so repeated lines of one product cannot oversell.
        var totals = items.GroupBy(i => i.ProductId).Select(g => (ProductId: g.Key, Quantity: g.Sum(i => i.Quantity))).ToList();
        lock (_lock)
        {
            if (!totals.All(t => IsAvailable(t.ProductId, t.Quantity))) return false;
            foreach (var t in totals)
                _products[t.ProductId] = _products[t.ProductId] with { Stock = _products[t.ProductId].Stock - t.Quantity };
            return true;
        }
    }

    public void Release(IEnumerable<OrderItem> items)
    {
        lock (_lock)
        {
            foreach (var i in items)
                _products[i.ProductId] = _products[i.ProductId] with { Stock = _products[i.ProductId].Stock + i.Quantity };
        }
    }
}
