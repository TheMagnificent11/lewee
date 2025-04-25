using FreeMediator;
using Lewee.Application.Mediation.Notifications;
using Lewee.Contracts;
using Lewee.Shared;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Lewee.Infrastructure.AspNet.SignalR;

internal class ClientEventHandler : INotificationHandler<ClientEvent>
{
    private readonly IHubContext<ClientEventHub> hubContext;
    private readonly ILogger logger;

    public ClientEventHandler(
        IHubContext<ClientEventHub> hubContext,
        ILogger<ClientEventHandler> logger)
    {
        this.hubContext = hubContext;
        this.logger = logger;
    }

    public async Task Handle(ClientEvent notification, CancellationToken cancellationToken)
    {
        using (this.logger.BeginScope(new Dictionary<string, object>
        {
            { LoggingConsts.CorrelationId, notification.CorrelationId }
        }))
        {
            var clientMessage = notification.ToClientMessage();

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
