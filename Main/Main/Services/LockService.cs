using Dapr.Client;

namespace Main.Services;

public class LockService : ILockService
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<ILockService> _logger;

    public LockService(DaprClient daprClient, ILogger<ILockService> logger)
    {
        _daprClient = daprClient;
        _logger = logger;
    }
    
    public async Task LockResource()
    {
        var fileLock = await _daprClient.Lock("lockstore", "resource_1", "main", 120);

        _logger.LogInformation(fileLock.Success ? "resource locked!" : "locking the resource hasn't worked.");
    }

    public async Task UnlockResource()
    {
        var lockResponse = await _daprClient.Unlock("lockstore", "resource_1", "main");
        
        _logger.LogInformation("lock status: {status}", lockResponse.status);
    }
}