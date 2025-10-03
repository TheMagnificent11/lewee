using Lewee.Domain;

namespace Pizzeria.Store.Domain;

public sealed class OrderPreparedEvent : DomainEvent
{
    public OrderPreparedEvent(Guid orderId, string userId, DateTime eventDateTime)
        : base()
    {
        this.OrderId = orderId;
        this.UserId = userId;
        this.EventDateTime = eventDateTime;
    }

    // Private parameterless constructor for JSON deserialization
    private OrderPreparedEvent()
    {
    }

    public Guid OrderId { get; init; }
    public DateTime EventDateTime { get; init; }
}
