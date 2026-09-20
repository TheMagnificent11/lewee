using Lewee.Infrastructure.Fluxor;
using Pizzeria.Store.Contracts.Orders;

namespace Pizzeria.Ordering.StateManagement.Orders.Actions;

public record StartOrderCompletedAction : IMessageReceivedAction<OrderDto>
{
    public Guid CorrelationId { get; init; }

    public OrderDto Data { get; init; }
}
