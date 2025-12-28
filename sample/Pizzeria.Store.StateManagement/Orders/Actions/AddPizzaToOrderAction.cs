using Lewee.Infrastructure.Fluxor;

namespace Pizzeria.Store.StateManagement.Orders.Actions;

public record AddPizzaToOrderAction : IRequestAction
{
    public Guid OrderId { get; init; }

    public Guid PizzaId { get; init; }

    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
