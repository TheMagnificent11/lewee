using Lewee.Contracts.StateManagement;

namespace Pizzeria.Store.Contracts.Orders.Actions;

public record StartOrderCompletedAction : IMessageReceivedAction
{
    public StartOrderCompletedAction(OrderDto order, Guid correlationId)
    {
        this.Order = order;
        this.CorrelationId = correlationId;
    }

    public OrderDto Order { get; init; }
    public Guid CorrelationId { get; init; }
}
