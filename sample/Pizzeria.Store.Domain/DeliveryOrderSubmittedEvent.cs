namespace Pizzeria.Store.Domain;

public sealed class DeliveryOrderSubmittedEvent : OrderSubmittedEvent
{
    public DeliveryOrderSubmittedEvent(
        Guid orderId,
        string userId,
        DateTime eventDateTime,
        string deliveryAddress,
        Guid correlationId)
        : base(orderId, userId, eventDateTime, correlationId)
    {
        this.DeliveryAddress = deliveryAddress;
    }

    public string DeliveryAddress { get; init; }
}
