using Lewee.Application.Mediation.Notifications;

namespace Lewee.Infrastructure.ServerEvents;

/// <summary>
/// Client Event Broadcaster Implementation
/// </summary>
/// <remarks>
/// This broadcaster uses events to broadcast client events to all subscribers.
/// Each Blazor circuit can subscribe to receive events and filter by user ID.
/// </remarks>
internal sealed class ClientEventBroadcaster : IClientEventBroadcaster
{
    /// <inheritdoc/>
    public event EventHandler<ClientEventArgs>? OnClientEvent;

    /// <inheritdoc/>
    public void Broadcast(ClientEvent clientEvent)
    {
        this.OnClientEvent?.Invoke(this, new ClientEventArgs(clientEvent));
    }
}
