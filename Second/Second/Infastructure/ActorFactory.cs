using Dapr.Actors;
using Dapr.Actors.Client;

namespace Second.Infastructure;

public class ActorFactory<TActor> : IActorFactory<TActor>
    where TActor : IActor
{
    public TActor CreateActor(string actorId)
    {
        var actorIds = new ActorId(actorId);
        var actorTypeName = typeof(TActor).Name[1..];

        return ActorProxy.Create<TActor>(actorIds, actorTypeName);
    }
}