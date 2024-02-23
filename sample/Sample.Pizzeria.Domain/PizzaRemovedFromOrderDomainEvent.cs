using Lewee.Domain;

namespace Sample.Pizzeria.Domain;

public class PizzaRemovedFromOrderDomainEvent : DomainEvent
{
    public PizzaRemovedFromOrderDomainEvent(Guid orderId, Guid orderPizzaId, Guid correlationId)
    {
        this.OrderId = orderId;
        this.OrderPizzaId = orderPizzaId;
        this.CorrelationId = correlationId;
    }

    public Guid OrderId { get; }
    public Guid OrderPizzaId { get; }
}
