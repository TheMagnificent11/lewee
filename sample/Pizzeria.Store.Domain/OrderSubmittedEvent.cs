using Lewee.Domain;

namespace Pizzeria.Store.Domain;

public abstract class OrderSubmittedEvent : DomainEvent
{
    protected OrderSubmittedEvent(Guid orderId, string userId, DateTime eventDateTime)
        : base()
    {
        this.OrderId = orderId;
        this.UserId = userId;
        this.EventDateTime = eventDateTime;
    }

    // Protected parameterless constructor for JSON deserialization
    protected OrderSubmittedEvent()
    {
    }

    public Guid OrderId { get; init; }
    public DateTime EventDateTime { get; init; }
}
