using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Blazor.Messaging;

/// <summary>
/// Event Re
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
        services.AddSingleton(new HubConnectionBuilder()
            .WithUrl($"{serverApiUrl}/events")
            .WithAutomaticReconnect()
            .Build());

        services.AddTransient<IMessageToActionMapper, TMapper>();
        services.AddTransient<MessageDeserializer>();

        return services;
    }
}
