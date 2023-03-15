using Lewee.Application.Mediation.Notifications;
using Lewee.Contracts;
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
        using (LogContext.PushProperty(LoggingConsts.ClientId, notification.ClientId))
        {
            var clientMessage = notification.ToClientMessage();

            // TODO: fix issue where client ID does not match SignalR connection ID
            // The issue appears to be how the controller gets the SignalR connection ID and passes it down.
            //var client = this.hubContext.Clients.Client(notification.ClientId);
            //if (client == null)
            //{
            //    this.logger.Information("Could not find SignalR client");
            //    return;
            //}

            //await client.SendAsync(nameof(ClientMessage), clientMessage, cancellationToken);
            //this.logger.Information("Published message to client");

            await this.hubContext.Clients.All.SendAsync(nameof(ClientMessage), clientMessage, cancellationToken);
            this.logger.Information("Published message to all clients");
        }
    }
}
