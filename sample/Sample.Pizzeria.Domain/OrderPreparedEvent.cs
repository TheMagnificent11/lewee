using Lewee.Domain;

namespace Sample.Pizzeria.Domain;

public class OrderPreparedDomainEvent : DomainEvent
{
    public OrderPreparedDomainEvent(Guid orderId, Guid correlationId)
    {
        this.OrderId = orderId;
        this.CorrelationId = correlationId;
    }

    public Guid OrderId { get; }
}
