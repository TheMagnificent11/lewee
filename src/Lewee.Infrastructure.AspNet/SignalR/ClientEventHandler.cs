using Lewee.Application.Mediation.Notifications;
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
            var client = this.hubContext.Clients.Client(notification.ClientId);
            if (client == null)
            {
                this.logger.Information("Could not find SignalR client {ClientId}", notification.ClientId);
                return;
            }

            await client.SendAsync("SendMessage", clientMessage, cancellationToken);
            this.logger.Information("Published message to client");
        }
    }
}
