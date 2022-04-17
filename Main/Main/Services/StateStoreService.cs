using Dapr.Client;
using Main.Models;

namespace Main.Services;

public class StateStoreService : IStateStoreService
{
    private const string Key = "123456";
    
    private readonly DaprClient _daprClient;

    public StateStoreService(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }
    
    public async Task<StateStoreDto> Get()
    {
        return await _daprClient.GetStateAsync<StateStoreDto>("statestore", Key);
    }

    public async Task<(StateStoreDto, string)> GetEtag()
    {
        return await _daprClient.GetStateAndETagAsync<StateStoreDto>("statestore", Key);
    }

    public async Task Set(StateStoreDto state)
    {
        await _daprClient.SaveStateAsync("statestore", Key, state);
    }

    public async Task Update(StateStoreEtagDto request)
    {
        await _daprClient.TrySaveStateAsync("statestore", Key, request.StateStore, request.ETag);
    }

    public async Task Delete()
    {
        await _daprClient.DeleteStateAsync("statestore", Key);
    }
}