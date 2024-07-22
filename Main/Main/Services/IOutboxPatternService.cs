using Main.Models;

namespace Main.Services;

public interface IOutboxPatternService
{
    Task ChangeState(OutboxDto body);
}