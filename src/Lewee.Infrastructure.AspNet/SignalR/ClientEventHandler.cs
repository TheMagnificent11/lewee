using Lewee.Application.Mediation.Notifications;
using Lewee.Contracts;
using Lewee.Shared;
using MediatR;
using Serilog;
using Serilog.Context;

namespace Lewee.Infrastructure.AspNet.SignalR;

internal class ClientEventHandler<T> : INotificationHandler<ClientEvent<T>>
    where T : IClientMessageContract
{
    private readonly ClientEventHub hub;
    private readonly ILogger logger;

    public ClientEventHandler(ClientEventHub hub, ILogger logger)
    {
        this.hub = hub;
        this.logger = logger.ForContext<ClientEventHandler<T>>();
    }

    public async Task Handle(ClientEvent<T> notification, CancellationToken cancellationToken)
    {
        using (LogContext.PushProperty(LoggingConsts.CorrelationId, notification.CorrelationId))
        {
            var clientMessage = notification.ToClientMessage();
            await this.hub.Publish(notification.ClientId, clientMessage, cancellationToken);
            this.logger.Information("Client message published (Message: {@Message})", notification.Message);
        }
    }
}
