using System.Diagnostics.CodeAnalysis;
using Lewee.Application.Mediation.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lewee.Infrastructure.ServerEvents;

internal sealed class ClientEventChannelHandler : INotificationHandler<ClientEvent>
{
    private readonly ConnectionManager connectionManager;
    private readonly ILogger<ClientEventChannelHandler> logger;

    public ClientEventChannelHandler(
        ConnectionManager connectionManager,
        ILogger<ClientEventChannelHandler> logger)
    {
        this.connectionManager = connectionManager;
        this.logger = logger;
    }

    public async Task Handle(
        [NotNull] ClientEvent notification,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(notification.UserId))
        {
            // TODO: Send to all user channels
            return;
        }

        var channelFound = this.connectionManager.TryGetChannelWriter(notification.UserId, out var channelWriter);
        if (!channelFound)
        {
            this.logger.LogNoUserEventsChannelFound(notification.UserId);
            return;
        }

        await channelWriter!.WriteAsync(notification, cancellationToken);
    }
}
