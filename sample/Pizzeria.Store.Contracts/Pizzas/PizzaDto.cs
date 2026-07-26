namespace Pizzeria.Store.Contracts.Pizzas;

public record PizzaDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price);
