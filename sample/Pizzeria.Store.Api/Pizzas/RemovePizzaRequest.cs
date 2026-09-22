namespace Pizzeria.Store.Api.Pizzas;

/// <summary>
/// Request for removing a menu pizza. The pizza id is bound from the route by FastEndpoints.
/// </summary>
internal sealed record RemovePizzaRequest
{
    public Guid PizzaId { get; init; }
}
