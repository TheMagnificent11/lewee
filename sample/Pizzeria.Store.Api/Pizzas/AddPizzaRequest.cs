namespace Pizzeria.Store.Api.Pizzas;

/// <summary>
/// Request body for adding a pizza to the menu.
/// </summary>
internal sealed record AddPizzaRequest
{
    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public decimal Price { get; init; }
}
