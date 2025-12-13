using Lewee.StateManagement;
using Pizzeria.Store.Contracts.Orders;

namespace Pizzeria.Store.StateManagement.Orders.Actions;

public record StartOrderCompletedAction : IMessageReceivedAction
{
    public Guid CorrelationId { get; init; }

    public OrderDto Order { get; init; }
}
