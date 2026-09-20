namespace Pizzeria.Store.Contracts.Orders;

public enum OrderStatus
{
    // In-progress order that has not yet been submitted (not displayed as a lifecycle status).
    InProgress = 0,

    // Submitted but not yet being made.
    Received = 1,

    // Being prepared in the kitchen.
    Making = 2,

    // Prepared pickup order awaiting collection.
    ReadyForPickup = 3,

    // Prepared delivery order awaiting dispatch.
    ReadyForDelivery = 4,

    // Delivery order out for delivery.
    Delivering = 5,

    // Order completed (picked up or delivered).
    Completed = 6,
}
