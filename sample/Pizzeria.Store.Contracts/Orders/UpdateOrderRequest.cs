namespace Pizzeria.Store.Contracts.Orders;

/// <summary>
/// Request body for updating an order.
/// The order id is bound from the route, not this body.
/// Exactly one of <see cref="AddPizzaId"/>, <see cref="RemovePizzaId"/>, <see cref="SubmitAsPickup"/> or
/// <see cref="SubmitAsDelivery"/> should be supplied per request.
/// </summary>
public sealed record UpdateOrderRequest
{
    /// <summary>
    /// Gets the ID of a pizza to add to the order.
    /// </summary>
    public Guid? AddPizzaId { get; init; }

    /// <summary>
    /// Gets the ID of a pizza to remove from the order.
    /// </summary>
    public Guid? RemovePizzaId { get; init; }

    /// <summary>
    /// Gets a value indicating whether the order should be submitted for pickup.
    /// </summary>
    public bool SubmitAsPickup { get; init; }

    /// <summary>
    /// Gets a value indicating whether the order should be submitted for delivery.
    /// </summary>
    public bool SubmitAsDelivery { get; init; }

    /// <summary>
    /// Gets the delivery address. Required when <see cref="SubmitAsDelivery"/> is <c>true</c>.
    /// </summary>
    public string? DeliveryAddress { get; init; }
}
