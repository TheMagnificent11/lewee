using Lewee.StateManagement;

namespace Pizzeria.Store.StateManagement.Pizzas.Actions;

public record LoadPizzasAction : IRequestAction
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
