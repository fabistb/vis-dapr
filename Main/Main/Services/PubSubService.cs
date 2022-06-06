using Dapr.Client;
using Main.Models;

namespace Main.Services;

public class PubSubService : IPubSubService
{
    private readonly DaprClient _daprClient;

    public PubSubService(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }
    
    public async Task PublishMessage(string topic, string message, string? version = null)
    {
        var messageDto = new PubSubMessageDto(version, message);
        await _daprClient.PublishEventAsync("messagebus", topic, messageDto);
    }
}