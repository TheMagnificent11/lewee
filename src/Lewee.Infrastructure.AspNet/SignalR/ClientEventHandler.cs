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
    private readonly ClientConnectionMapper clientConnectionMapper;
    private readonly ILogger logger;

    public ClientEventHandler(IHubContext<ClientEventHub> hubContext, ClientConnectionMapper clientConnectionMapper, ILogger logger)
    {
        this.hubContext = hubContext;
        this.clientConnectionMapper = clientConnectionMapper;
        this.logger = logger.ForContext<ClientEventHandler>();
    }

    public async Task Handle(ClientEvent notification, CancellationToken cancellationToken)
    {
        using (LogContext.PushProperty(LoggingConsts.CorrelationId, notification.CorrelationId))
        using (LogContext.PushProperty(LoggingConsts.ClientId, notification.ClientId))
        {
            var clientMessage = notification.ToClientMessage();
            var connections = this.clientConnectionMapper.GetConnections(notification.ClientId);

            if (!connections.Any())
            {
                this.logger.Debug("No SignalR connections associated with client");
                return;
            }

            foreach (var connectionId in connections)
            {
                using (LogContext.PushProperty(LoggingConsts.SignalRConnectionId, connectionId))
                {
                    var client = this.hubContext.Clients.Client(connectionId);
                    if (client == null)
                    {
                        this.logger.Debug("Could not find SignalR client");
                        return;
                    }

                    await client.SendAsync(nameof(ClientMessage), clientMessage, cancellationToken);
                    this.logger.Debug("Published message to client");
                }
            }

            //await this.hubContext.Clients.All.SendAsync(nameof(ClientMessage), clientMessage, cancellationToken);
            //this.logger.Information("Published message to all clients");
        }
    }
}
