namespace Pizzeria.Store.Api.Orders;

/// <summary>
/// Request for submitting an order.
/// The order id is bound from the route by FastEndpoints.
/// </summary>
internal sealed record SubmitOrderRequest
{
    public Guid OrderId { get; init; }

    public string? DeliveryAddress { get; init; }
}
