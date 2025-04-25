namespace Pizzeria.Store.Contracts;

public record PizzaDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price);
