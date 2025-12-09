using Lewee.Contracts.StateManagement;

namespace Pizzeria.Store.Contracts.Orders.Actions;

public record AddPizzaToOrderCompletedAction : IMessageReceivedAction
{
    public AddPizzaToOrderCompletedAction(OrderDto order, Guid correlationId)
    {
        this.Order = order;
        this.CorrelationId = correlationId;
    }

    public OrderDto Order { get; init; }
    public Guid CorrelationId { get; init; }
}
