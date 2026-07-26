namespace Pizzeria.Store.Api.Orders;

/// <summary>
/// Request for adding a pizza to an order.
/// Route parameters are bound to the properties by FastEndpoints.
/// </summary>
internal sealed record AddPizzaToOrderRequest
{
    public Guid OrderId { get; init; }

    public Guid PizzaId { get; init; }
}
