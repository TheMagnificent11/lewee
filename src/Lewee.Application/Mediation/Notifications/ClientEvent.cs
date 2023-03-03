using MediatR;

namespace Lewee.Application.Mediation.Notifications;

/// <summary>
/// Client Event
/// </summary>
/// <typeparam name="T">Message type</typeparam>
public class ClientEvent<T> : INotification
    where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientEvent{T}"/> class
    /// </summary>
    /// <param name="correlationId">Correlation ID</param>
    /// <param name="clientId">SignalR Client ID</param>
    /// <param name="message">Message</param>
    public ClientEvent(Guid correlationId, string clientId, T message)
    {
        this.CorrelationId = correlationId;
        this.ClientId = clientId;
        this.Message = message;
    }

    /// <summary>
    /// Gets the correlation ID
    /// </summary>
    public Guid CorrelationId { get; }

    /// <summary>
    /// Gets the ID of the client to which the SignalR message will be sent
    /// </summary>
    public string ClientId { get; }

    /// <summary>
    /// Gets the message
    /// </summary>
    public T Message { get; }
}
