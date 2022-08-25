using Saji.Domain;

namespace Sample.Customers.Domain.Events;

public class UserSetEvent : IDomainEvent
{
    public UserSetEvent(
        Guid correlationId,
        Guid customerId,
        string userId)
    {
        this.CorrelationId = correlationId;
        this.CustomerId = customerId;
        this.UserId = userId;
    }

    public Guid CorrelationId { get; }

    public Guid CustomerId { get; }

    public string UserId { get; }
}
