using Lewee.Domain;
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
    /// <returns>The updated services collection</returns>
    public static IServiceCollection ConfigureSignalR(this IServiceCollection services)
    {
        services.AddSignalR();
        services.AddHttpContextAccessor();
        services.AddSingleton<IClientService, SignalRClientService>();

        return services;
    }
}
