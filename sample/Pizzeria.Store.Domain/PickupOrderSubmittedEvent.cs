namespace Pizzeria.Store.Domain;

public class PickupOrderSubmittedEvent : OrderSubmittedEvent
{
    public PickupOrderSubmittedEvent(
        Guid orderId,
        string userId,
        DateTime eventDateTime,
        Guid correlationId)
        : base(orderId, userId, eventDateTime, correlationId)
    {
    }
}
