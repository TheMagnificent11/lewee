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
    private const string SignalRHttpClientName = "LeweeSignalR";

    /// <summary>
    /// Configures application to receive messages over SignalR
    /// </summary>
    /// <typeparam name="TMapper">Mapper type</typeparam>
    /// <param name="services">Services collection</param>
    /// <param name="serverBaseAddress">Server base address</param>
    /// <param name="httpMessageHandler">Optional HTTP message handler for testing scenarios</param>
    /// <returns>Updated services collection</returns>
    public static IServiceCollection AddAzureSignalRMessageReceiver<TMapper>(
        this IServiceCollection services,
        Uri serverBaseAddress,
        HttpMessageHandler? httpMessageHandler = null)
        where TMapper : class, IMessageToActionMapper
    {
        // Configure base path so the SignalR client will POST to /signalr/negotiate
        var hubUri = serverBaseAddress.AppendPathSegment("signalr");

        if (httpMessageHandler != null)
        {
            // For testing scenarios, build the HubConnection directly with the provided handler
            var hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUri.ToString(), options =>
                {
                    options.HttpMessageHandlerFactory = _ => httpMessageHandler;
                    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
                })
                .WithAutomaticReconnect()
                .Build();

            services.AddSingleton(hubConnection);
        }
        else
        {
            // Register a named HttpClient for SignalR that will use service discovery (configured via ConfigureHttpClientDefaults)
            services.AddHttpClient(SignalRHttpClientName, client =>
            {
                client.BaseAddress = serverBaseAddress;
            });

            // Register HubConnection as a factory to build it at runtime with service discovery
            services.AddSingleton(sp =>
            {
                var httpMessageHandlerFactory = sp.GetRequiredService<IHttpMessageHandlerFactory>();

                var hubConnectionBuilder = new HubConnectionBuilder()
                    .WithUrl(hubUri.ToString(), options =>
                    {
                        options.HttpMessageHandlerFactory = _ =>
                            httpMessageHandlerFactory.CreateHandler(SignalRHttpClientName);
                    })
                    .WithAutomaticReconnect();

                return hubConnectionBuilder.Build();
            });
        }

        services
            .AddTransient<IMessageToActionMapper, TMapper>()
            .AddTransient<MessageDeserializer>()
            .AddHttpClient<HealthCheckService>(client => client.BaseAddress = serverBaseAddress);

        return services;
    }

    /// <summary>
    /// Configures application to receive messages over SignalR using service discovery
    /// </summary>
    /// <typeparam name="TMapper">Mapper type</typeparam>
    /// <param name="services">Services collection</param>
    /// <param name="apiAspireServiceName">Name of the Aspire API service to connect to</param>
    /// <returns>Updated services collection</returns>
    public static IServiceCollection AddAzureSignalRMessageReceiver<TMapper>(
        this IServiceCollection services,
        string apiAspireServiceName)
        where TMapper : class, IMessageToActionMapper
    {
        var apiUri = new Uri($"https://{apiAspireServiceName}");

        return services.AddAzureSignalRMessageReceiver<TMapper>(apiUri);
    }

    /// <summary>
    /// Add SignalR Message Receiver
    /// </summary>
    /// <typeparam name="TMapper">Mapper type</typeparam>
    /// <param name="services">Services collection</param>
    /// <returns>Updated services collection</returns>
    public static IServiceCollection AddSignalRMessageReceiver<TMapper>(
        this IServiceCollection services)
        where TMapper : class, IMessageToActionMapper
    {
        services
            .AddTransient<IMessageToActionMapper, TMapper>()
            .AddTransient<MessageDeserializer>();

        return services;
    }
}
