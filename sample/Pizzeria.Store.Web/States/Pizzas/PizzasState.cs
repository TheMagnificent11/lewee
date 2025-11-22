using Pizzeria.Store.Contracts;

namespace Pizzeria.Store.Web.States.Pizzas;

internal record PizzasState
{
    public PizzaDto[] Pizzas { get; init; } = [];
    public bool IsLoading { get; init; }
    public string? ErrorMessage { get; init; }
}
