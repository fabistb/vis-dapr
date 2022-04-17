namespace Main.Models;

public class MathActorState
{
    public int CurrentValue { get; set; }
    
    public string? LastOperation { get; set; }

    public DateTime? LastReminderCall { get; set; }
}