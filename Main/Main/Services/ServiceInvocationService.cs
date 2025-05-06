using System.Text;
using System.Text.Json;
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

    public async Task InvokeExternal(ServiceInvocationDto request)
    {
        var orderJson = JsonSerializer.Serialize(request);
        var content = new StringContent(orderJson, Encoding.UTF8, "application/json");
        
        var httpClient = DaprClient.CreateInvokeHttpClient();
        var response = await httpClient.PostAsJsonAsync("http://localhost:5071/api/v1.0/service-invocation", content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("calling external service failed");
        }
    }
}