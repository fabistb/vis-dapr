using Workflow.Models;

namespace Workflow.Services;

public interface IInMemoryInventory
{
    bool IsAvailable(string productId, int quantity);
    decimal? GetUnitPrice(string productId);
    bool TryReserve(string orderId, IEnumerable<OrderItem> items);
    void Release(string orderId);
}
