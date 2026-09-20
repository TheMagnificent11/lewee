namespace Pizzeria.Store.Api.Orders;

/// <summary>
/// Request that carries only the order id, bound from the route by FastEndpoints.
/// </summary>
internal sealed record OrderIdRequest
{
    public Guid OrderId { get; init; }
}
