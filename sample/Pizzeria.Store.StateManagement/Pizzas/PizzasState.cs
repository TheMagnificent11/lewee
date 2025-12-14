using Lewee.StateManagement;
using Pizzeria.Store.Contracts.Pizzas;

namespace Pizzeria.Store.StateManagement.Pizzas;

public record PizzasState : RequestState
{
    public IEnumerable<PizzaDto> Pizzas { get; init; } = [];

    public bool IsLoading { get; init; }
}
