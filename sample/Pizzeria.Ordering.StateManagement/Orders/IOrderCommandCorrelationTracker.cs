namespace Pizzeria.Ordering.StateManagement.Orders;

/// <summary>
/// Tracks the <see cref="OrderCommandType"/> associated with an in-flight order command's correlation ID so that
/// the SSE message, received when the command completes, can be mapped back to the correct completed Fluxor action.
/// This is required because every order command's SSE message carries the same <c>OrderDto</c> payload type, so the
/// correlation ID is the only way to distinguish which command the message relates to.
/// </summary>
public interface IOrderCommandCorrelationTracker
{
    /// <summary>
    /// Registers the command type associated with a correlation ID
    /// </summary>
    /// <param name="correlationId">Correlation ID</param>
    /// <param name="commandType">Order command type</param>
    void Register(Guid correlationId, OrderCommandType commandType);

    /// <summary>
    /// Attempts to retrieve and remove the command type associated with a correlation ID
    /// </summary>
    /// <param name="correlationId">Correlation ID</param>
    /// <param name="commandType">Order command type, if found</param>
    /// <returns><c>true</c> if the correlation ID was found; otherwise, <c>false</c></returns>
    bool TryGetAndRemove(Guid correlationId, out OrderCommandType commandType);
}
