using Dapr.Actors.Runtime;
using Second.Models;

namespace Second.Actors;

public class ResiliencyActor : Actor, IResiliencyActor, IRemindable
{
    private const string ReminderName = "ResiliencyActor";
    private const string StateName = "actorstate";

    public ResiliencyActor(
        ActorHost host,
        IActorStateManager? stateManager = null) 
        : base(host)
    {
        if (stateManager != null)
        {
            StateManager = stateManager;
        }
    }

    public async Task SimulateExhaustedResource()
    {
        var state = await StateManager.TryGetStateAsync<ResiliencyState>(StateName);

        if (state.HasValue)
        {
            state.Value.CallCount++;

            if (state.Value.CallCount >= 200)
            {
                throw new SystemException("Application has reach it's rate limit");
            }
        }
        else
        {
            var newState = new ResiliencyState
            {
                CallCount = 0,
            };

            await StateManager.SetStateAsync(StateName, newState);
            await StateManager.SaveStateAsync();
            await RegisterReminderAsync(ReminderName, null, TimeSpan.FromMinutes(2), TimeSpan.FromMilliseconds(-1));
        }
    }
    
    public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
    {
        await StateManager.RemoveStateAsync(StateName);
        await StateManager.SaveStateAsync();
        await UnregisterReminderAsync(ReminderName);
    }
}