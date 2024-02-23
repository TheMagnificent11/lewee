using Lewee.Domain;

namespace Sample.Pizzeria.Domain;

public class OrderCreatedDomainEvent : DomainEvent
{
    public OrderCreatedDomainEvent(Guid orderId, Guid correlationId)
    {
        this.OrderId = orderId;
        this.CorrelationId = correlationId;
    }

    public Guid OrderId { get; }
}
