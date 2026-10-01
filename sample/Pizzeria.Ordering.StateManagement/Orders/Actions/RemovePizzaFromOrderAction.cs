using Lewee.Infrastructure.Fluxor;

namespace Pizzeria.Ordering.StateManagement.Orders.Actions;

public record RemovePizzaFromOrderAction : IRequestAction
{
    public Guid OrderId { get; init; }

    public Guid PizzaId { get; init; }

    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
