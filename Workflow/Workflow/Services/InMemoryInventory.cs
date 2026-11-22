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

    private readonly Dictionary<string, Dictionary<string, int>> _reservations = [];

    // Guards the multi-item reserve/release so an order is all-or-nothing.
    private readonly object _lock = new();

    public bool IsAvailable(string productId, int quantity) =>
        _products.TryGetValue(productId, out var p) && p.Stock >= quantity;

    public decimal? GetUnitPrice(string productId) =>
        _products.TryGetValue(productId, out var p) ? p.Price : null;

    public bool TryReserve(string orderId, IEnumerable<OrderItem> items)
    {
        // Sum per product so repeated lines of one product cannot oversell.
        var totals = items.GroupBy(i => i.ProductId).ToDictionary(g => g.Key, g => g.Sum(i => i.Quantity));
        lock (_lock)
        {
            if (_reservations.ContainsKey(orderId)) return true; // replayed activity: already reserved
            if (!totals.All(t => IsAvailable(t.Key, t.Value))) return false;
            foreach (var (productId, quantity) in totals)
                _products[productId] = _products[productId] with { Stock = _products[productId].Stock - quantity };
            _reservations[orderId] = totals;
            return true;
        }
    }

    public void Release(string orderId)
    {
        lock (_lock)
        {
            if (!_reservations.Remove(orderId, out var totals)) return; // nothing reserved (e.g. after a restart)
            foreach (var (productId, quantity) in totals)
                _products[productId] = _products[productId] with { Stock = _products[productId].Stock + quantity };
        }
    }
}
