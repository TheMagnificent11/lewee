using System.Threading.Channels;
using Lewee.Application.Mediation.Notifications;
using MediatR;

namespace Lewee.Infrastructure.Fluxor;

/// <summary>
/// Writes client events to the channel for SSE broadcasting
/// </summary>
internal sealed class ClientEventChannelHandler : INotificationHandler<ClientEvent>
{
    private readonly ChannelWriter<ClientEvent> channelWriter;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientEventChannelHandler"/> class
    /// </summary>
    /// <param name="channelWriter">Channel writer</param>
    public ClientEventChannelHandler(ChannelWriter<ClientEvent> channelWriter)
    {
        this.channelWriter = channelWriter;
    }

    /// <inheritdoc/>
    public async Task Handle(ClientEvent notification, CancellationToken cancellationToken)
    {
        await this.channelWriter.WriteAsync(notification, cancellationToken);
    }
}
