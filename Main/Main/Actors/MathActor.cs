using Dapr.Actors.Runtime;
using Main.Models;

namespace Main.Actors;

public class MathActor : Actor, IMathActor, IRemindable
{
    private readonly ILogger<MathActor> _logger;

    public MathActor(
        ILogger<MathActor> logger,
        ActorHost host,
        IActorStateManager? stateManager = null) 
        : base(host)
    {
        _logger = logger;
        
        if (stateManager != null)
        {
            StateManager = stateManager;
        }
    }
    
    public async Task SetActorState(MathActorState state)
    {
        await StateManager.SetStateAsync("statestore", state);
        await RegisterReminderAsync(
            "mathreminder",
            null,
            TimeSpan.FromMinutes(1),
            TimeSpan.FromMinutes(5));
    }

    public async Task<int> Addition(int value)
    {
        var actorState = await StateManager.TryGetStateAsync<MathActorState>("statestore");
        if (!actorState.HasValue)
        {
            throw new Exception("no actor state present");
        }

        actorState.Value.CurrentValue += value;
        actorState.Value.LastOperation = "addition";

        return actorState.Value.CurrentValue;
    }

    public async Task<int> Substraction(int value)
    {
        var actorState = await StateManager.TryGetStateAsync<MathActorState>("statestore");
        if (!actorState.HasValue)
        {
            throw new Exception("no actor state present");
        }

        actorState.Value.CurrentValue -= value;
        actorState.Value.LastOperation = "substraction";

        return actorState.Value.CurrentValue;
    }

    public async Task<MathActorState> Get()
    {
        return await StateManager.GetStateAsync<MathActorState>("statestore");
    }

    public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
    {
        var actorState = await StateManager.TryGetStateAsync<MathActorState>("statestore");
        actorState.Value.LastReminderCall = DateTime.UtcNow;
        _logger.LogInformation($"Reminder Triggered. Current value: {actorState.Value.CurrentValue}");
    }
}