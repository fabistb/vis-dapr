using Dapr.Client;
using Main.Models;

namespace Main.Services;

public class ServiceInvocationService : IServiceInvocationService
{
    private readonly DaprClient _daprClient;

    public ServiceInvocationService(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }
    
    public async Task Invoke(ServiceInvocationDto request)
    {
        var daprHttpClient = DaprClient.CreateInvokeHttpClient();
        var response = await daprHttpClient.PostAsJsonAsync("http://second/api/v1.0/service-invocation", request);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("service invocation failed.");
        }
    }
}