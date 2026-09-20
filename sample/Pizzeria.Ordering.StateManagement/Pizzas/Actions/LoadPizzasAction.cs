using Lewee.Infrastructure.Fluxor;

namespace Pizzeria.Ordering.StateManagement.Pizzas.Actions;

public record LoadPizzasAction : IRequestAction
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
