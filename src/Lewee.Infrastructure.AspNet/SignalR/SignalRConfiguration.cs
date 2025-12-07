using System.Text.Json;
using System.Text.Json.Serialization;
using Lewee.Application.Mediation.Notifications;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.AspNet.SignalR;

/// <summary>
/// SignalR Configuration
/// </summary>
public static class SignalRConfiguration
{
    /// <summary>
    /// Configures SignalR
    /// </summary>
    /// <param name="services">Services collection</param>
    /// <param name="connectionString">Azure SignalR connection string (serverless SignalR, not default)</param>
    /// <returns>The updated services collection</returns>
    public static IServiceCollection AddLeweeSignalR(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddSingleton(sp =>
        {
            return new ServiceManagerBuilder()
                .WithOptions(options => options.ConnectionString = connectionString)
                .BuildServiceManager();
        });

        services.AddMediatR(config => config.RegisterServicesFromAssemblies(
            typeof(ClientEvent).Assembly,
            typeof(ClientEventHandler).Assembly));

        return services;
    }

    /// <summary>
    /// Maps the SignalR negotiate endpoint
    /// </summary>
    /// <param name="app">Web application builder</param>
    /// <returns>The updated web application builder</returns>
    public static WebApplication MapLeweeSignalRNegotiateEndpoint(this WebApplication app)
    {
        app.MapPost("/negotiate", async (string? userId, ServiceManager sm, CancellationToken token) =>
        {
            // The creation of the ServiceHubContext is expensive, so it's recommended to
            // only create it once per named context / per app run if possible.
            var context = await sm.CreateHubContextAsync<ClientEventHub>("events", token);

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
