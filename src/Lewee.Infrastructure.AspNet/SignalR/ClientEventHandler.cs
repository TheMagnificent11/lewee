using System.Diagnostics.CodeAnalysis;
using Lewee.Application.Mediation.Notifications;
using Lewee.Common;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Lewee.Infrastructure.AspNet.SignalR;

[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes",
    Justification = "Use via DI")]
internal sealed class ClientEventHandler : INotificationHandler<ClientEvent>
{
    private readonly IHubContext<ClientEventHub> hubContext;
    private readonly ClientEventChannel eventChannel;
    private readonly ILogger logger;

    public ClientEventHandler(
        IHubContext<ClientEventHub> hubContext,
        ClientEventChannel eventChannel,
        ILogger<ClientEventHandler> logger)
    {
        this.hubContext = hubContext;
        this.eventChannel = eventChannel;
        this.logger = logger;
    }

    public async Task Handle(ClientEvent notification, CancellationToken cancellationToken)
    {
        using (this.logger.BeginScope(new Dictionary<string, object>(StringComparer.Ordinal)
        {
            { LoggingConsts.CorrelationId, notification.CorrelationId },
        }))
        {
            var clientMessage = notification.ToClientMessage();

            // Write to channel for Blazor Server circuits to consume
            await this.eventChannel.Writer.WriteAsync(clientMessage, cancellationToken);
            this.logger.LogDebug("Client event written to channel");

            // Also send via SignalR hub for any external clients
            if (string.IsNullOrWhiteSpace(notification.UserId))
            {
                await this.hubContext
                    .Clients
                    .All
                    .SendAsync(nameof(ClientMessage), clientMessage, cancellationToken);

                this.logger.LogDebug("Published message to all clients");

                return;
            }

            await this.hubContext
                .Clients
                .Group(notification.UserId)
                .SendAsync(nameof(ClientMessage), clientMessage, cancellationToken);

            this.logger.LogDebug("Published message to specific client(s)");
        }
    }
}
