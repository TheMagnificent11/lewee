using Pizzeria.Store.Contracts;

namespace Pizzeria.Store.Web.States.Pizzas;

public record PizzasState
{
    public IEnumerable<PizzaDto> Pizzas { get; init; } = [];
    public bool IsLoading { get; init; }
    public string? ErrorMessage { get; init; }
}
