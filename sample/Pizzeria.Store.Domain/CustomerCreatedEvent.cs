using Lewee.Domain;

namespace Pizzeria.Store.Domain;

public sealed class CustomerCreatedEvent : DomainEvent
{
    public CustomerCreatedEvent(
        Guid customerId,
        string externalId,
        Guid correlationId)
        : base(correlationId)
    {
        this.CustomerId = customerId;
        this.ExternalId = externalId;
    }

    public Guid CustomerId { get; init; }
    public string ExternalId { get; init; }
}
