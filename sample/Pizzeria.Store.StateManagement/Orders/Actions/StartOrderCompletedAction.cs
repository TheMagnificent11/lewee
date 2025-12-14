using Lewee.StateManagement;
using Pizzeria.Store.Contracts.Orders;

namespace Pizzeria.Store.StateManagement.Orders.Actions;

public record StartOrderCompletedAction : IMessageReceivedAction<OrderDto>
{
    public Guid CorrelationId { get; init; }

    public OrderDto Data { get; init; }
}
