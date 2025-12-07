using Lewee.Application.Mediation.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.AspNet.SignalR;

/// <summary>
/// SignalR Configuration
/// </summary>
public static class SignalRConfiguration
{
    /// <summary>
    /// Configures SignalR
    /// </summary>
    /// <param name="services">Services collection</param>
    /// <param name="aspireSignalRServiceName">Aspire SignalR service name</param>
    /// <returns>The updated services collection</returns>
    public static IServiceCollection AddLeweeSignalR(
        this IServiceCollection services,
        string? aspireSignalRServiceName = null)
    {
        if (string.IsNullOrWhiteSpace(aspireSignalRServiceName))
        {
            services.AddSignalR();
        }
        else
        {
            services.AddSignalR().AddNamedAzureSignalR(aspireSignalRServiceName);
        }

        services.AddMediatR(config => config.RegisterServicesFromAssemblies(
            typeof(ClientEvent).Assembly,
            typeof(ClientEventHandler).Assembly));

        return services;
    }
}
