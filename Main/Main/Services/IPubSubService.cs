namespace Main.Services;

public interface IPubSubService
{
    Task PublishMessage(string message);
}