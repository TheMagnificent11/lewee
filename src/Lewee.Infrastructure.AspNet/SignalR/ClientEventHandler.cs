using Lewee.Contracts;
using Lewee.Domain;
using Lewee.Shared;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Serilog.Context;

namespace Lewee.Infrastructure.AspNet.SignalR;

internal class ClientEventHandler : INotificationHandler<ClientEvent>
{
    private readonly IHubContext<ClientEventHub> hubContext;
    private readonly ILogger logger;

    public ClientEventHandler(IHubContext<ClientEventHub> hubContext, ILogger logger)
    {
        this.hubContext = hubContext;
        this.logger = logger.ForContext<ClientEventHandler>();
    }

    public async Task Handle(ClientEvent notification, CancellationToken cancellationToken)
    {
        using (LogContext.PushProperty(LoggingConsts.CorrelationId, notification.CorrelationId))
        {
            var clientMessage = notification.ToClientMessage();

            if (string.IsNullOrWhiteSpace(notification.UserId))
            {
                await this.hubContext
                    .Clients
                    .All
                    .SendAsync(nameof(ClientMessage), clientMessage, cancellationToken);

                this.logger.Debug("Published message to all clients");

                return;
            }

            await this.hubContext
                .Clients
                .Group(notification.UserId)
                .SendAsync(nameof(ClientMessage), clientMessage, cancellationToken);

            this.logger.Debug("Published message to specific client(s)");
        }
    }
}
