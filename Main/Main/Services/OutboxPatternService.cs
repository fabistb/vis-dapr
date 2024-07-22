using System.Text.Json;
using Dapr.Client;
using Main.Models;

namespace Main.Services;

public class OutboxPatternService : IOutboxPatternService
{
    private readonly DaprClient _daprClient;

    public OutboxPatternService(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }
    
    public async Task ChangeState(OutboxDto body)
    {
        await _daprClient.ExecuteStateTransactionAsync("outboxstatestore", new[]
        {
            new StateTransactionRequest(
                body.Key,
                ObjectToJsonByteArray(body),
                StateOperationType.Upsert)
        });
    }
    
    private static byte [] ObjectToJsonByteArray<T>(T obj)
    {
        var jsonString = JsonSerializer.Serialize(obj);
        return System.Text.Encoding.UTF8.GetBytes(jsonString);
    }
}