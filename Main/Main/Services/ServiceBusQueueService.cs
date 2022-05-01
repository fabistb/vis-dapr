using System.Text.Json;
using Dapr.Client;
using Main.Models;

namespace Main.Services;

public class ServiceBusQueueService : IServiceBusQueueService
{
    private readonly DaprClient _daprClient;

    public ServiceBusQueueService(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }
    
    public async Task AddMessage(ServiceBusMessageDto message)
    {
        var bindingRequest = new BindingRequest("servicebusqueue", "create")
        {
            Data = JsonSerializer.SerializeToUtf8Bytes(message),
        };

        await _daprClient.InvokeBindingAsync(bindingRequest);
    }
}