using Lewee.StateManagement;
using Pizzeria.Store.Contracts.Orders;

namespace Pizzeria.Store.StateManagement.Orders.Actions;

public record StartOrderCompletedAction(OrderDto Order, Guid CorrelationId)
    : IMessageReceivedAction;
