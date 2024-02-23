using Lewee.Domain;

namespace Sample.Pizzeria.Domain;

public class OrderPickedUpForDeliveryDomainEvent : DomainEvent
{
    public OrderPickedUpForDeliveryDomainEvent(Guid orderId, Guid correlationId)
    {
        this.OrderId = orderId;
        this.CorrelationId = correlationId;
    }

    public Guid OrderId { get; }
}
