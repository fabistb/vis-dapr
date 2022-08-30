using Dapr.Actors;

namespace Second.Actors;

public interface IResiliencyActor : IActor
{
    Task SimulateExhaustedResource();
}