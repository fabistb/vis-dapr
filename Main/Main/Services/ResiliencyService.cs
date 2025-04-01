using Dapr.Client;

namespace Main.Services;

public class ResiliencyService : IResiliencyService
{
    private const int NumbCalls = 300;
    
    private readonly DaprClient _daprClient;
    private readonly ILogger<ResiliencyService> _logger;

    public ResiliencyService(
        DaprClient daprClient,
        ILogger<ResiliencyService> logger)
    {
        _daprClient = daprClient;
        _logger = logger;
    }
    
    public async Task ExhaustResource()
    { 
        var daprHttpClient = DaprClient.CreateInvokeHttpClient();
        
        for (var i = 0; i < NumbCalls; i++)
        {
            var response = await daprHttpClient.PostAsync($"http://second/api/v1.0/resiliency?Guid={Guid.NewGuid()}", null);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Resiliency policy didn't worked as expected.");
            }
        }
    }
}