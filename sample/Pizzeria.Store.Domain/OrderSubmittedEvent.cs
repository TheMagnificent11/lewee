using Lewee.Domain;

namespace Pizzeria.Store.Domain;

public abstract class OrderSubmittedEvent : DomainEvent
{
    protected OrderSubmittedEvent(
        Guid orderId,
        string userId,
        DateTime eventDateTime,
        Guid correlationId)
        : base(correlationId)
    {
        this.OrderId = orderId;
        this.UserId = userId;
        this.EventDateTime = eventDateTime;
    }

    public Guid OrderId { get; init; }
    public DateTime EventDateTime { get; init; }
}
