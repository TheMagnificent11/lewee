using Lewee.StateManagement;

namespace Pizzeria.Store.StateManagement.Orders.Actions;

public record AddPizzaToOrderAction(Guid OrderId, Guid PizzaId) : IRequestAction
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
