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

    public Guid OrderId { get; }
    public DateTime StartedDateTime { get; }
}
