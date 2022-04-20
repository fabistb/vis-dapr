using Dapr.Actors;
using Main.Models;

namespace Main.Actors;

public interface IMathActor : IActor
{
    Task SetActorState(MathActorState state);

    Task<int> Addition(int value);

    Task<int> Substraction(int value);

    Task<MathActorState> Get();
}