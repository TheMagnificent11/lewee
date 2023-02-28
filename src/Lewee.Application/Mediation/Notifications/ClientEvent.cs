using MediatR;

namespace Lewee.Application.Mediation.Notifications;

/// <summary>
/// Client Event
/// </summary>
/// <typeparam name="T">Message type</typeparam>
public class ClientEvent<T> : INotification
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientEvent{T}"/> class
    /// </summary>
    /// <param name="clientId">SignalR Client ID</param>
    /// <param name="message">Message</param>
    public ClientEvent(string clientId, T message)
    {
        this.ClientId = clientId;
        this.Message = message;
    }

    /// <summary>
    /// Gets the ID of the client to which the SignalR message will be sent
    /// </summary>
    public string ClientId { get; }

    /// <summary>
    /// Gets the message
    /// </summary>
    public T Message { get; }
}
