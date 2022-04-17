using Main.Models;

namespace Main.Services;

public interface IServiceInvocationService
{
    Task Invoke(ServiceInvocationDto request);
}