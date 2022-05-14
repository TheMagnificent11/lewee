using Saji.Domain;

namespace Sample.Customers.Contracts.Events;

public class CustomerCreatedEvent : IDomainEvent
{
    public CustomerCreatedEvent(
        Guid correlationId,
        Guid customerId,
        string givenName,
        string surname)
    {
        this.CorrelationId = correlationId;
        this.CustomerId = customerId;
        this.GivenName = givenName;
        this.Surname = surname;
    }

    public Guid CorrelationId { get; protected set; }

    public Guid CustomerId { get; protected set; }

    public string GivenName { get; protected set; }

    public string Surname { get; protected set; }
}
