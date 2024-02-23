using Lewee.Domain;

namespace Sample.Pizzeria.Domain;

public class OrderDeliverDomainEvent : DomainEvent
{
    public OrderDeliverDomainEvent(Guid orderId, Guid correlationId)
    {
        this.OrderId = orderId;
        this.CorrelationId = correlationId;
    }

    public Guid OrderId { get; }
}
