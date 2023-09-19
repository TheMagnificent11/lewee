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
    /// <param name="serverBaseAddress">Server base address</param>
    /// <returns>Updated services collection</returns>
    public static IServiceCollection ConfigureMessageReceiver<TMapper>(
        this IServiceCollection services,
        string serverBaseAddress)
        where TMapper : class, IMessageToActionMapper
    {
        var hubUrl = serverBaseAddress.EndsWith('/') ? $"{serverBaseAddress}events" : $"{serverBaseAddress}/events";
        var hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        services
            .AddSingleton(hubConnection)
            .AddTransient<IMessageToActionMapper, TMapper>()
            .AddTransient<MessageDeserializer>()
            .AddHttpClient<HealthCheckService>(sp => sp.BaseAddress = new Uri(serverBaseAddress));

        return services;
    }
}
