using System.Text.Json;
using System.Text.Json.Serialization;
using Lewee.Application.Mediation.Notifications;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.AspNet.SignalR;

/// <summary>
/// SignalR Configuration
/// </summary>
public static class SignalRConfiguration
{
    internal const string EventsHubName = "events";

    /// <summary>
    /// Configures SignalR
    /// </summary>
    /// <param name="services">Services collection</param>
    /// <returns>The updated services collection</returns>
    public static IServiceCollection AddLeweeSignalR(this IServiceCollection services)
    {
        services.AddSignalR();
        services.AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/octet-stream"]);
        });
        services.AddTransient<INotificationHandler<ClientEvent>, ClientEventHandler>();

        return services;
    }

    /// <summary>
    /// Configures Azure SignalR
    /// </summary>
    /// <param name="services">Services collection</param>
    /// <param name="connectionString">Azure SignalR connection string (serverless SignalR, not default)</param>
    /// <returns>The updated services collection</returns>
    public static IServiceCollection AddLeweeAzureSignalR(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddSingleton(sp =>
        {
            return new ServiceManagerBuilder()
                .WithOptions(options => options.ConnectionString = connectionString)
                .BuildServiceManager();
        });

        services.AddTransient<INotificationHandler<ClientEvent>, AzureSignalRClientEventHandler>();

        return services;
    }

    /// <summary>
    /// Maps the SignalR hub endpoint
    /// </summary>
    /// <param name="app">Web application</param>
    /// <returns>The updated web application</returns>
    public static WebApplication MapLeweeSignalRHub(this WebApplication app)
    {
        app.MapHub<ClientEventHub>(ClientEventHub.HubPath);

        return app;
    }

    /// <summary>
    /// Maps the SignalR negotiate endpoint
    /// </summary>
    /// <param name="app">Web application builder</param>
    /// <returns>The updated web application builder</returns>
    public static WebApplication MapLeweeAzureSignalRNegotiateEndpoint(this WebApplication app)
    {
        // SignalR client will POST to /signalr/negotiate when configured with base /signalr
        app.MapPost("/signalr/negotiate", async (string? userId, ServiceManager sm, CancellationToken token) =>
        {
            // Use non-generic CreateHubContextAsync - the generic version expects a client interface type
            var context = await sm.CreateHubContextAsync(EventsHubName, token);

            var negotiateResponse = await context.NegotiateAsync(
                new NegotiationOptions
                {
                    UserId = userId,
                },
                token);

            // The JSON serializer options need to be set to ignore null values, otherwise the
            // response will contain null values for the properties that are not set.
            // The .NET SignalR client will not be able to parse the response if the null values are present.
            // For more information, see https://github.com/dotnet/aspnetcore/issues/60935.
            return Results.Json(negotiateResponse, new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            });
        });

        return app;
    }
}
