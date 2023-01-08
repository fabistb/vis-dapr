using Dapr.Client;
using Second.Models;

namespace Second.Services;

public class StateStoreService : IStateStoreService
{
    private const string Key = "123456";
    
    private readonly DaprClient _daprClient;

    public StateStoreService(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }
    
    public async Task<StateStoreDto> GetSharedState()
    {
        var result = await _daprClient.GetStateAsync<StateStoreDto>("sharedstatestore", Key);

        return result;
    }
}