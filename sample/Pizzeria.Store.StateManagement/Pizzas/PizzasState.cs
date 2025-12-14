using Lewee.StateManagement;
using Pizzeria.Store.Contracts.Pizzas;

namespace Pizzeria.Store.StateManagement.Pizzas;

public record PizzasState : CommandState
{
    public IEnumerable<PizzaDto> Pizzas { get; init; } = [];
}
