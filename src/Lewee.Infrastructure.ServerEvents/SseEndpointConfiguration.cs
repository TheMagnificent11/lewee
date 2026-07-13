using System.Net.ServerSentEvents;
using System.Security.Claims;
using Lewee.Application.Mediation.Notifications;
using Lewee.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Lewee.Infrastructure.ServerEvents;

/// <summary>
/// SSE Endpoint Configuration
/// </summary>
public static class SseEndpointConfiguration
{
    /// <summary>
    /// SSE events endpoint path
    /// </summary>
    public const string EventsEndpoint = "/events";

    /// <summary>
    /// Maps the SSE endpoint for client events
    /// </summary>
    /// <param name="app">Web application</param>
    /// <returns>The updated web application</returns>
    public static WebApplication MapSseEndpoint(this WebApplication app)
    {
        app.MapGet(
            EventsEndpoint,
            async (
                ConnectionManager connectionManager,
                HttpContext httpContext,
                CancellationToken cancellationToken) =>
        {
            var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var userChannel = connectionManager.GetOrCreateChannel(userId);

            async IAsyncEnumerable<SseItem<ClientMessage>> StreamEventsAsync()
            {
                var enumerator = userChannel.Reader.ReadAllAsync(cancellationToken).GetAsyncEnumerator(cancellationToken);
                try
                {
                    while (true)
                    {
                        try
                        {
                            if (!await enumerator.MoveNextAsync())
                            {
                                break;
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            // Client disconnected - stop enumeration
                            break;
                        }

                        var clientEvent = enumerator.Current;
                        var clientMessage = clientEvent.ToClientMessage();
                        yield return new SseItem<ClientMessage>(clientMessage, clientEvent.ContractFullClassName);
                    }
                }
                finally
                {
                    await enumerator.DisposeAsync();
                    connectionManager.RemoveChannel(userId);
                }
            }

            return TypedResults.ServerSentEvents(StreamEventsAsync(), "events");
        })
        .RequireAuthorization();

        return app;
    }
}
