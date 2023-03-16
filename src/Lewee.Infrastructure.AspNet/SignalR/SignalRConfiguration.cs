using Lewee.Application.Mediation.Notifications;
using Lewee.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
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
        services.AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                new[] { "application/octet-stream" });
        });
        services.AddHttpContextAccessor();
        services.AddSingleton<IClientService, SignalRClientService>();
        services.AddSingleton<ClientConnectionMapper>();
        services.AddMediatR(config => config.RegisterServicesFromAssemblies(
            typeof(ClientEvent).Assembly,
            typeof(ClientEventHandler).Assembly));

        return services;
    }
}
