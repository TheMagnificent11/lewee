using Lewee.Infrastructure.Fluxor;
using Pizzeria.Store.Contracts.Pizzas;

namespace Pizzeria.Ordering.StateManagement.Pizzas.Actions;

public record LoadPizzasSuccessAction : IQuerySuccessAction<IEnumerable<PizzaDto>>
{
    public Guid CorrelationId { get; init; }

    public IEnumerable<PizzaDto> Data { get; init; } = [];
}
