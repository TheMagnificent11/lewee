using System.Diagnostics.CodeAnalysis;
using Lewee.Application.Mediation.Notifications;
using Lewee.Contracts;
using Lewee.Shared;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Extensions.Logging;

namespace Lewee.Infrastructure.AspNet.SignalR;

[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes",
    Justification = "Use via DI")]
internal sealed class ClientEventHandler : INotificationHandler<ClientEvent>
{
    private readonly ServiceManager serviceManager;
    private readonly ILogger logger;

    public ClientEventHandler(
        ServiceManager serviceManager,
        ILogger<ClientEventHandler> logger)
    {
        this.serviceManager = serviceManager;
        this.logger = logger;
    }

    public async Task Handle(ClientEvent notification, CancellationToken cancellationToken)
    {
        using (this.logger.BeginScope(new Dictionary<string, object>(StringComparer.Ordinal)
        {
            { LoggingConsts.CorrelationId, notification.CorrelationId },
        }))
        {
            var hubContext = await this.serviceManager.CreateHubContextAsync(
                SignalRConfiguration.EventsHubName,
                cancellationToken);
            var clientMessage = notification.ToClientMessage();

            if (string.IsNullOrWhiteSpace(notification.UserId))
            {
                await hubContext
                    .Clients
                    .All
                    .SendAsync(nameof(ClientMessage), clientMessage, cancellationToken);

                this.logger.LogDebug("Published message to all clients");

                return;
            }

            await hubContext
                .Clients
                .User(notification.UserId)
                .SendAsync(nameof(ClientMessage), clientMessage, cancellationToken);

            this.logger.LogDebug("Published message to user {UserId}", notification.UserId);
        }
    }
}
