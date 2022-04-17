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
    
    public async Task PublishMessage(string message)
    {
        var messageDto = new PubSubMessageDto(message);
        await _daprClient.PublishEventAsync("messagebus", "pub-sub-receive", messageDto);
    }
}