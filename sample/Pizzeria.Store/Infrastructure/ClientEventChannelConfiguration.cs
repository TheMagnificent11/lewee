using System.Threading.Channels;
using Lewee.Application.Mediation.Notifications;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Pizzeria.Store.Infrastructure;

/// <summary>
/// Client Event Channel Configuration
/// </summary>
internal static class ClientEventChannelConfiguration
{
    /// <summary>
    /// Adds the client event channel for SSE broadcasting
    /// </summary>
    /// <param name="services">Services collection</param>
    /// <returns>The updated services collection</returns>
    public static IServiceCollection AddClientEventChannel(this IServiceCollection services)
    {
        var channel = Channel.CreateUnbounded<ClientEvent>(new UnboundedChannelOptions
        {
            SingleReader = false,
            SingleWriter = false,
        });

        services.AddSingleton(channel);
        services.AddSingleton(channel.Reader);
        services.AddSingleton(channel.Writer);

        // Register the handler that writes to the channel
        services.AddTransient<INotificationHandler<ClientEvent>, ClientEventChannelHandler>();

        return services;
    }
}
