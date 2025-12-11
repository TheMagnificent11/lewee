using Lewee.StateManagement;
using Pizzeria.Store.Contracts.Orders;

namespace Pizzeria.Store.StateManagement.Orders.Actions;

public record AddPizzaToOrderCompletedAction(OrderDto Order, Guid CorrelationId)
    : IMessageReceivedAction
{
}
