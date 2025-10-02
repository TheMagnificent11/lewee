using Pizzeria.Store.Contracts;

namespace Pizzeria.Store.WebClient.States.Pizzas;

public record PizzasState
{
    public PizzaDto[] Pizzas { get; init; } = [];
    public bool IsLoading { get; init; } = false;
    public string? ErrorMessage { get; init; }
}
