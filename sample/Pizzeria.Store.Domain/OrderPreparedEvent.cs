using Lewee.Domain;

namespace Pizzeria.Store.Domain;

public sealed class OrderPreparedEvent : DomainEvent
{
    public OrderPreparedEvent(
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
}
