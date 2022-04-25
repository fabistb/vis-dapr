using Dapr.Client;

namespace Main.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly DaprClient _daprClient;

    public ConfigurationService(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }
    
    public async Task<GetConfigurationResponse> Configuration()
    {
        var result = await _daprClient.GetConfiguration("configuration", new[] {"orderId1", "orderId2"});
        return result;
    }
}