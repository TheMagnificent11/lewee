using Lewee.Application.Mediation.Notifications;
using Lewee.Shared;
using MediatR;
using Serilog;
using Serilog.Context;

namespace Lewee.Infrastructure.AspNet.SignalR;

internal class ClientEventHandler : INotificationHandler<ClientEvent>
{
    private readonly ClientEventHub hub;

    public ClientEventHandler(ClientEventHub hub, ILogger logger)
    {
        this.hub = hub;
    }

    public async Task Handle(ClientEvent notification, CancellationToken cancellationToken)
    {
        using (LogContext.PushProperty(LoggingConsts.CorrelationId, notification.CorrelationId))
        {
            var clientMessage = notification.ToClientMessage();
            await this.hub.Publish(notification.ClientId, clientMessage, cancellationToken);
        }
    }
}
