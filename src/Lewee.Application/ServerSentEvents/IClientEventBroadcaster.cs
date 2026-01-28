using Lewee.Application.Mediation.Notifications;

namespace Lewee.Application.ServerSentEvents;

/// <summary>
/// Client Event Broadcaster Interface
/// </summary>
public interface IClientEventBroadcaster
{
    /// <summary>
    /// Event raised when a client event is broadcast
    /// </summary>
    event EventHandler<ClientEventArgs>? OnClientEvent;

    /// <summary>
    /// Broadcasts a client event to all subscribers
    /// </summary>
    /// <param name="clientEvent">Client event to broadcast</param>
    void Broadcast(ClientEvent clientEvent);
}
