namespace Main.Services;

public interface IPubSubService
{
    Task PublishMessage(string topic, string message, string? version = null);
}