using Dapr.Client;

namespace Main.Services;

public interface IConfigurationService
{
    Task<GetConfigurationResponse> Configuration();
}