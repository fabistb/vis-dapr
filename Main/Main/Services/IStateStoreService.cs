using Main.Models;

namespace Main.Services;

public interface IStateStoreService
{
    Task<StateStoreDto> Get();

    Task<(StateStoreDto, string)> GetEtag();

    Task Set(StateStoreDto state);

    Task<bool> Update(StateStoreEtagDto request);

    Task Delete();
}