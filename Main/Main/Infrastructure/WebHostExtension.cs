using Dapr.Client;
using Dapr.Extensions.Configuration;

namespace Main.Infrastructure;

public static class WebHostExtension
{
    public static IWebHostBuilder RegisterDaprClientAndSecretStore(this IWebHostBuilder webHostBuilder)
    {
        var client = new DaprClientBuilder().Build();

        return webHostBuilder
            .ConfigureServices(services =>
            {
                if (services.Any(s => s.ImplementationType == typeof(DaprClientMarker)))
                {
                    return;
                }

                services.AddSingleton<DaprClientMarker>();
                services.AddSingleton(client);
            })
            .ConfigureAppConfiguration((ctx, builder) =>
            {
                if (!ctx.HostingEnvironment.IsDevelopment())
                {
                    builder.AddDaprSecretStore("secret-store", client);
                }
            });
    }

    private class DaprClientMarker
    {
    }
}