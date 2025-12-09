namespace Lewee.Contracts.StateManagement;

/// <summary>
/// Message Received Action Interface
/// </summary>
public interface IMessageReceivedAction
{
    /// <summary>
    /// Gets the correlation ID
    /// </summary>
    Guid CorrelationId { get; }
}
