using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
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
            var writeTasks = this.connectionManager
                .GetAllChannelWriters()
                .Select(writer => this.WriteToChannelAsync(writer, notification, cancellationToken));

            await Task.WhenAll(writeTasks);
            return;
        }

        var channelFound = this.connectionManager.TryGetChannelWriter(notification.UserId, out var channelWriter);
        if (!channelFound)
        {
            this.logger.LogNoUserEventsChannelFound(notification.UserId);
            return;
        }

        await this.WriteToChannelAsync(channelWriter!, notification, cancellationToken);
    }

    private async Task WriteToChannelAsync(
        ChannelWriter<ClientEvent> writer,
        ClientEvent notification,
        CancellationToken cancellationToken)
    {
        try
        {
            await writer.WriteAsync(notification, cancellationToken);
        }
        catch (ChannelClosedException ex)
        {
            this.logger.LogChannelClosedOnWrite(ex);
        }
    }
}
