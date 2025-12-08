using Flurl;
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
    /// <param name="httpMessageHandler">Optional HTTP message handler for testing scenarios</param>
    /// <returns>Updated services collection</returns>
    public static IServiceCollection AddMessageReceiver<TMapper>(
        this IServiceCollection services,
        Uri serverBaseAddress,
        HttpMessageHandler? httpMessageHandler = null)
        where TMapper : class, IMessageToActionMapper
    {
        // Configure base path so the SignalR client will POST to /signalr/negotiate
        var hubUri = serverBaseAddress.AppendPathSegment("signalr");
        var hubConnectionBuilder = new HubConnectionBuilder()
            .WithUrl(hubUri.ToString(), options =>
            {
                if (httpMessageHandler != null)
                {
                    // For testing scenarios, use the provided message handler and long polling
                    options.HttpMessageHandlerFactory = _ => httpMessageHandler;
                    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
                }
            })
            .WithAutomaticReconnect();

        var hubConnection = hubConnectionBuilder.Build();

        services
            .AddSingleton(hubConnection)
            .AddTransient<IMessageToActionMapper, TMapper>()
            .AddTransient<MessageDeserializer>()
            .AddHttpClient<HealthCheckService>(sp => sp.BaseAddress = serverBaseAddress);

        return services;
    }

    /// <summary>
    /// Configures application to receive messages over SignalR using service discovery
    /// </summary>
    /// <typeparam name="TMapper">Mapper type</typeparam>
    /// <param name="services">Services collection</param>
    /// <param name="apiAspireServiceName">Name of the Aspire API service to connect to</param>
    /// <returns>Updated services collection</returns>
    public static IServiceCollection AddMessageReceiver<TMapper>(
        this IServiceCollection services,
        string apiAspireServiceName)
        where TMapper : class, IMessageToActionMapper
    {
        var apiUri = new Uri($"https://{apiAspireServiceName}");

        return services.AddMessageReceiver<TMapper>(apiUri);
    }
}
