namespace Pizzeria.Store.Domain;

public class PickupOrderSubmittedEvent : OrderSubmittedEvent
{
    public PickupOrderSubmittedEvent(Guid orderId, string userId, DateTime eventDateTime)
        : base(orderId, userId, eventDateTime)
    {
    }
}
