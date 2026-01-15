using System.Threading.Channels;
using Lewee.Application.Mediation.Notifications;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.ServerEvents;

/// <summary>
/// Client Event Channel Configuration
/// </summary>
public static class ClientEventChannelConfiguration
{
    /// <summary>
    /// Adds the client event broadcaster for SSE broadcasting
    /// </summary>
    /// <param name="services">Services collection</param>
    /// <returns>The updated services collection</returns>
    public static IServiceCollection AddClientEventBroadcaster(this IServiceCollection services)
    {
        var channel = Channel.CreateUnbounded<ClientEvent>(new UnboundedChannelOptions
        {
            SingleReader = false,
            SingleWriter = false,
        });

        services.AddSingleton(channel);
        services.AddSingleton(channel.Reader);
        services.AddSingleton(channel.Writer);

        services.AddTransient<INotificationHandler<ClientEvent>, ClientEventChannelHandler>();

        return services;
    }
}
