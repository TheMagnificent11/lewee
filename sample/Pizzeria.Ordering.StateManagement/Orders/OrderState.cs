using Lewee.Infrastructure.Fluxor;
using Pizzeria.Store.Contracts.Orders;

namespace Pizzeria.Ordering.StateManagement.Orders;

public record OrderState : RequestState<OrderDto>
{
    /// <summary>
    /// Gets the type of order command currently in flight (identified by <see cref="IRequestState{T}.CorrelationId"/>).
    /// Used to map the SSE message received when the command completes back to the correct completed Fluxor action,
    /// since every order command's SSE message carries the same <see cref="OrderDto"/> payload type.
    /// </summary>
    public OrderCommandType? PendingCommandType { get; init; }
}
