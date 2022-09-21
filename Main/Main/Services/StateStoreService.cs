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

    public async Task<bool> Update(StateStoreEtagDto request)
    {
        var response = await _daprClient.TrySaveStateAsync("statestore", Key, request.StateStore, request.ETag);
        return response;
    }

    public async Task Delete()
    {
        await _daprClient.DeleteStateAsync("statestore", Key);
    }

    public async Task<List<QueryStoreDto>> FilterByFirstName()
    {
        const string query = "{" +
                             "\"filter\": {" +
                             "\"EQ\": { \"firstName\": \"Max\" }" +
                             "}" +
                             "}";

        var queryResult = await _daprClient.QueryStateAsync<QueryStoreDto>("querystore", query);

        return queryResult.Results.Select(result => result.Data).ToList();
    }
}