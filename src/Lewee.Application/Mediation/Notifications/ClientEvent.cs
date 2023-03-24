using System.Text.Json;
using Lewee.Contracts;
using MediatR;

namespace Lewee.Application.Mediation.Notifications;

/// <summary>
/// Client Event
/// </summary>
public class ClientEvent : INotification
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientEvent"/> class
    /// </summary>
    /// <param name="correlationId">Correlation ID</param>
    /// <param name="userId">User ID</param>
    /// <param name="message">Message</param>
    public ClientEvent(Guid correlationId, string? userId, object message)
    {
        this.CorrelationId = correlationId;
        this.UserId = userId;

        var messageType = message.GetType();

        this.ContractAssemblyName = messageType.Assembly.FullName ?? string.Empty;
        this.ContractFullClassName = messageType.FullName ?? string.Empty;
        this.MessageJson = JsonSerializer.Serialize(message, messageType);
    }

    /// <summary>
    /// Gets the correlation ID
    /// </summary>
    public Guid CorrelationId { get; }

    /// <summary>
    /// Gets the ID of the user ID
    /// </summary>
    public string? UserId { get; }

    /// <summary>
    /// Gets the assembly name of the JSON contract class
    /// </summary>
    public string ContractAssemblyName { get; }

    /// <summary>
    /// Gets the full class name of the JSON contract class
    /// </summary>
    public string ContractFullClassName { get; }

    /// <summary>
    /// Gets the message JSON
    /// </summary>
    public string MessageJson { get; }

    /// <summary>
    /// Converts to a client message
    /// </summary>
    /// <returns>A client message</returns>
    public ClientMessage ToClientMessage()
    {
        return new ClientMessage
        {
            CorrelationId = this.CorrelationId,
            ContractAssemblyName = this.ContractAssemblyName,
            ContractFullClassName = this.ContractFullClassName,
            MessageJson = this.MessageJson
        };
    }
}
