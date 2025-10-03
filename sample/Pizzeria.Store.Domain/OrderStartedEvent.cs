using Lewee.Domain;

namespace Pizzeria.Store.Domain;

public sealed class OrderStartedEvent : DomainEvent
{
    public OrderStartedEvent(Guid orderId, string userId, DateTime eventDateTime)
        : base()
    {
        this.OrderId = orderId;
        this.UserId = userId;
        this.StartedDateTime = eventDateTime;
    }

    // Private parameterless constructor for JSON deserialization
    private OrderStartedEvent()
    {
    }

    public Guid OrderId { get; init; }
    public DateTime StartedDateTime { get; init; }
}
