namespace Lewee.Domain;

/// <summary>
/// To Client Event Interface
/// </summary>
/// <remarks>
/// Add this interface to <see cref="DomainEvent"/> implementations that are to be sent to the client.
/// </remarks>
public interface IToClientEvent
{
    /// <summary>
    /// Converts to a client event
    /// </summary>
    /// <param name="correlationId">Correlation ID</param>
    /// <param name="userId">User ID</param>
    /// <returns>A client event object</returns>
    ClientEvent ToClientEvent(Guid correlationId, string? userId);
}
