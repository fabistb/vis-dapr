namespace Second.Infastructure;

public interface IActorFactory<TActor>
{
    TActor CreateActor(string actorId);
}