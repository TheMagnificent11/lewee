using Lewee.StateManagement;
using Pizzeria.Store.Contracts.Pizzas;

namespace Pizzeria.Store.StateManagement.Pizzas.Actions;

public record LoadPizzasSuccessAction : IQuerySuccessAction<IEnumerable<PizzaDto>>
{
    public Guid CorrelationId { get; init; }

    public IEnumerable<PizzaDto> Data { get; init; } = [];
}
