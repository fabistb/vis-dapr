using Second.Models;

namespace Second.Services;

public interface IStateStoreService
{
    Task<StateStoreDto> GetSharedState();
}