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
        var httpRequest = _daprClient.CreateInvokeMethodRequest(HttpMethod.Post, "second", "api/v1.0/receive", request);
        var response = await _daprClient.InvokeMethodWithResponseAsync(httpRequest);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("service invocation failed.");
        }
    }
}