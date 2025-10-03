namespace Pizzeria.Store.Domain;

public sealed class DeliveryOrderSubmittedEvent : OrderSubmittedEvent
{
    public DeliveryOrderSubmittedEvent(
        Guid orderId,
        string userId,
        DateTime eventDateTime,
        string deliveryAddress)
        : base(orderId, userId, eventDateTime)
    {
        this.DeliveryAddress = deliveryAddress;
    }

    // Private parameterless constructor for JSON deserialization
    private DeliveryOrderSubmittedEvent()
    {
    }

    public string DeliveryAddress { get; init; } = string.Empty;
}
