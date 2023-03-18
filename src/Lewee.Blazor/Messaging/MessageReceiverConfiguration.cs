using Lewee.Blazor.Messaging.Health;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Blazor.Messaging;

/// <summary>
/// Message Receiver Configuration
/// </summary>
public static class MessageReceiverConfiguration
{
    /// <summary>
    /// Configures application to receive messages over SignalR
    /// </summary>
    /// <typeparam name="TMapper">Mapper type</typeparam>
    /// <param name="services">Services collection</param>
    /// <param name="serverApiUrl">Server API URL</param>
    /// <returns>Updated services collection</returns>
    public static IServiceCollection ConfigureMessageReceiver<TMapper>(
        this IServiceCollection services,
        string serverApiUrl)
        where TMapper : class, IMessageToActionMapper
    {
        var hubConnection = new HubConnectionBuilder()
            .WithUrl($"{serverApiUrl}/events")
            .WithAutomaticReconnect()
            .Build();

        services
            .AddSingleton(hubConnection)
            .AddTransient<IMessageToActionMapper, TMapper>()
            .AddTransient<MessageDeserializer>()
            .AddTransient<HealthCheckService>();

        return services;
    }
}
