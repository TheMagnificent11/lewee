using Lewee.Domain;

namespace Sample.Pizzeria.Domain;

public class OrderPlacedDomainEvent : DomainEvent
{
    public OrderPlacedDomainEvent(Guid orderId, Guid correlationId)
    {
        this.OrderId = orderId;
        this.CorrelationId = correlationId;
    }

    public Guid OrderId { get; }
}
