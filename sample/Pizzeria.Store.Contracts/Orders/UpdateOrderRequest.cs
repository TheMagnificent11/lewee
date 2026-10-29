namespace Pizzeria.Store.Contracts.Orders;

/// <summary>
/// Request body for updating an order.
/// The order id is bound from the route, not this body.
/// Exactly one order update action should be supplied per request.
/// </summary>
public sealed record UpdateOrderRequest
{
    /// <summary>
    /// Maximum allowed length for <see cref="DeliveryAddress"/>.
    /// Must match <c>Pizzeria.Store.Domain.Order.FieldLengths.DeliveryAddress</c>.
    /// </summary>
    public const int DeliveryAddressMaxLength = 250;

    /// <summary>
    /// Gets a value indicating whether the order should be submitted for pickup.
    /// </summary>
    public bool SubmitAsPickup { get; init; }

    /// <summary>
    /// Gets a value indicating whether the order should be submitted for delivery.
    /// </summary>
    public bool SubmitAsDelivery { get; init; }

    public OrderFulfillmentAction? FulfillmentAction { get; init; }

    /// <summary>
    /// Gets the delivery address. Required when <see cref="SubmitAsDelivery"/> is <c>true</c>.
    /// </summary>
    public string? DeliveryAddress { get; init; }
}
