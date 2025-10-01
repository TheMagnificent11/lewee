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
    /// <returns>Updated services collection</returns>
    public static IServiceCollection AddMessageReceiver<TMapper>(
        this IServiceCollection services,
        Uri serverBaseAddress)
        where TMapper : class, IMessageToActionMapper
    {
        return services.AddMessageReceiver<TMapper>(serverBaseAddress, httpMessageHandler: null);
    }

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
        HttpMessageHandler? httpMessageHandler)
        where TMapper : class, IMessageToActionMapper
    {
        var hubUri = serverBaseAddress.AppendPathSegment("events");
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
    /// <param name="httpClientName">Name of the HttpClient configured with service discovery</param>
    /// <returns>Updated services collection</returns>
    public static IServiceCollection AddMessageReceiverWithServiceDiscovery<TMapper>(
        this IServiceCollection services,
        string httpClientName)
        where TMapper : class, IMessageToActionMapper
    {
        // Register HubConnection as a factory that uses the named HttpClient
        services.AddSingleton<HubConnection>(serviceProvider =>
        {
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(httpClientName);
            
            if (httpClient.BaseAddress == null)
            {
                throw new InvalidOperationException($"HttpClient '{httpClientName}' must have a BaseAddress configured for service discovery");
            }

            var hubUri = httpClient.BaseAddress.AppendPathSegment("events");
            
            return new HubConnectionBuilder()
                .WithUrl(hubUri.ToString(), options =>
                {
                    // Use a factory that creates the service discovery handler for each connection
                    options.HttpMessageHandlerFactory = _ => new ServiceDiscoveryHttpMessageHandler(httpClientFactory, httpClientName);
                })
                .WithAutomaticReconnect()
                .Build();
        });

        services
            .AddTransient<IMessageToActionMapper, TMapper>()
            .AddTransient<MessageDeserializer>()
            .AddHttpClient<HealthCheckService>(httpClientName); // Use the same named client for health checks

        return services;
    }
}

/// <summary>
/// HTTP message handler that uses service discovery through a named HttpClient
/// </summary>
internal sealed class ServiceDiscoveryHttpMessageHandler : HttpMessageHandler
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly string httpClientName;

    public ServiceDiscoveryHttpMessageHandler(IHttpClientFactory httpClientFactory, string httpClientName)
    {
        this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        this.httpClientName = httpClientName ?? throw new ArgumentNullException(nameof(httpClientName));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Create a new HttpClient instance for each request to ensure proper service discovery
        using var httpClient = this.httpClientFactory.CreateClient(this.httpClientName);
        
        // Create a new request message to avoid "request already sent" errors
        using var newRequest = new HttpRequestMessage(request.Method, request.RequestUri);
        
        // Copy headers
        foreach (var header in request.Headers)
        {
            newRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        
        // Copy content if present
        if (request.Content != null)
        {
            var contentBytes = await request.Content.ReadAsByteArrayAsync(cancellationToken);
            newRequest.Content = new ByteArrayContent(contentBytes);
            
            // Copy content headers
            foreach (var header in request.Content.Headers)
            {
                newRequest.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }
        
        // Copy properties
        foreach (var property in request.Options)
        {
            newRequest.Options.Set(new HttpRequestOptionsKey<object?>(property.Key), property.Value);
        }
        
        // Forward the request through the named HttpClient which uses service discovery
        return await httpClient.SendAsync(newRequest, cancellationToken);
    }
}
