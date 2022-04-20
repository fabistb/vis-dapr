using Dapr.Client;

namespace Main.Services;

public class SecretStoreService : ISecretStoreService
{
    private readonly DaprClient _daprClient;

    public SecretStoreService(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }
    
    public async Task<string> GetSecretBySecretName(string secretName)
    {
        var resultDic = await _daprClient.GetSecretAsync("secretstore", secretName);
        return resultDic.First().Value;
    }

    public async Task<Dictionary<string, Dictionary<string, string>>> GetSecrets()
    {
        return await _daprClient.GetBulkSecretAsync("secretstore");
    }
}