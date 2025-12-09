namespace Pizzeria.Store.Contracts.Pizzas.Actions;

public record LoadPizzasAction
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
