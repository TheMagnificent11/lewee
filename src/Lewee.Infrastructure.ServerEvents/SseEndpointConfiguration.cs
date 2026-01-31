using System.Net.ServerSentEvents;
using System.Security.Claims;
using System.Threading.Channels;
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
                ChannelReader<ClientEvent> channelReader,
                HttpContext httpContext,
                CancellationToken cancellationToken) =>
        {
            var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            async IAsyncEnumerable<SseItem<ClientMessage>> StreamEventsAsync()
            {
                var enumerator = channelReader.ReadAllAsync(cancellationToken).GetAsyncEnumerator(cancellationToken);
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
                        if (clientEvent.UserId == userId)
                        {
                            var clientMessage = clientEvent.ToClientMessage();
                            yield return new SseItem<ClientMessage>(clientMessage, clientEvent.ContractFullClassName);
                        }
                    }
                }
                finally
                {
                    await enumerator.DisposeAsync();
                }
            }

            return TypedResults.ServerSentEvents(StreamEventsAsync(), "events");
        })
        .RequireAuthorization();

        return app;
    }
}
