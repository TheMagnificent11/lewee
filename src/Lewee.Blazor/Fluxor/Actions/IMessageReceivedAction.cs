namespace Lewee.Blazor.Fluxor.Actions;

/// <summary>
/// Message Received Action Interface
/// </summary>
public interface IMessageReceivedAction
{
    /// <summary>
    /// Gets the correlation ID
    /// </summary>
    Guid CorrelationId { get; }

    /// <summary>
    /// Gets the request type
    /// </summary>
    string RequestType { get; }
}
