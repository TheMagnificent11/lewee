namespace Pizzeria.Store.StateManagement.Pizzas.Actions;

public record LoadPizzasAction
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
