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

/// <summary>
/// Client Event Args
/// </summary>
public class ClientEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientEventArgs"/> class
    /// </summary>
    /// <param name="clientEvent">Client event</param>
    public ClientEventArgs(ClientEvent clientEvent)
    {
        this.ClientEvent = clientEvent;
    }

    /// <summary>
    /// Gets the client event
    /// </summary>
    public ClientEvent ClientEvent { get; }
}
