using Workflow.Models;

namespace Workflow.Services;

public interface IInMemoryInventory
{
    bool IsAvailable(string productId, int quantity);
    decimal? GetUnitPrice(string productId);
    bool TryReserve(IEnumerable<OrderItem> items);
    void Release(IEnumerable<OrderItem> items);
}
