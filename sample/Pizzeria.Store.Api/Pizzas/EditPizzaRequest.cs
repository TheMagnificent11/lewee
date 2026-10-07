namespace Pizzeria.Store.Api.Pizzas;

/// <summary>
/// Request for editing a menu pizza. The pizza id is bound from the route by FastEndpoints.
/// </summary>
internal sealed record EditPizzaRequest
{
    public Guid PizzaId { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public decimal Price { get; init; }
}
