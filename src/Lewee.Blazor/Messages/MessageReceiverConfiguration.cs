﻿using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Blazor.Messages;

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
        services.AddSingleton<IMessageToActionMapper, TMapper>();
        services.AddTransient<MessageDeserializer>();
        services.AddTransient(_ => new MessageReceiver($"{serverApiUrl}/events"));
        services.AddSingleton<MessageReceiverService>();

        return services;
    }
}
