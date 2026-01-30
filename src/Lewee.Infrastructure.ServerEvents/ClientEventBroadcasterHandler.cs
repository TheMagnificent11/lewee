using Lewee.Application.Mediation.Notifications;
using MediatR;

namespace Lewee.Infrastructure.ServerEvents;

/// <summary>
/// Client Event Broadcaster Handler
/// </summary>
/// <remarks>
/// Handles <see cref="ClientEvent"/> notifications by broadcasting them to all subscribers.
/// </remarks>
internal sealed class ClientEventBroadcasterHandler : INotificationHandler<ClientEvent>
{
    private readonly IClientEventBroadcaster clientEventBroadcaster;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientEventBroadcasterHandler"/> class
    /// </summary>
    /// <param name="clientEventBroadcaster">Client event broadcaster</param>
    public ClientEventBroadcasterHandler(IClientEventBroadcaster clientEventBroadcaster)
    {
        this.clientEventBroadcaster = clientEventBroadcaster;
    }

    /// <inheritdoc/>
    public Task Handle(ClientEvent notification, CancellationToken cancellationToken)
    {
        this.clientEventBroadcaster.Broadcast(notification);
        return Task.CompletedTask;
    }
}
