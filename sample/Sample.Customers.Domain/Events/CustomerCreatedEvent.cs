using Saji.Domain;

namespace Sample.Customers.Domain.Events;

public class CustomerCreatedEvent : IDomainEvent
{
    public CustomerCreatedEvent(
        Guid correlationId,
        Guid customerId)
    {
        this.CorrelationId = correlationId;
        this.CustomerId = customerId;
    }

    public Guid CorrelationId { get; protected set; }

    public Guid CustomerId { get; protected set; }
}
