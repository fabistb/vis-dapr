using Dapr.Actors;
using Dapr.Actors.Client;

namespace Second.Infastructure;

public class ActorFactory<TActor> : IActorFactory<TActor>
    where TActor : IActor
{
    private readonly IActorProxyFactory _actorProxyFactory;

    public ActorFactory(IActorProxyFactory actorProxyFactory)
    {
        _actorProxyFactory = actorProxyFactory;
    }

    public TActor CreateActor(string actorId)
    {
        var actorIds = new ActorId(actorId);
        var actorTypeName = typeof(TActor).Name[1..];

        return _actorProxyFactory.CreateActorProxy<TActor>(actorIds, actorTypeName);
    }
}