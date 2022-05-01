using Main.Models;

namespace Main.Services;

public interface IServiceBusQueueService
{
    Task AddMessage(ServiceBusMessageDto message);
}