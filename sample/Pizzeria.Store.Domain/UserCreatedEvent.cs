using Lewee.Domain;

namespace Pizzeria.Store.Domain;

public sealed class UserCreatedEvent : DomainEvent
{
    public UserCreatedEvent(
        Guid userEntityId,
        string externalId,
        Guid correlationId)
        : base(correlationId)
    {
        this.UserEntityId = userEntityId;
        this.ExternalId = externalId;
    }

    public Guid UserEntityId { get; init; }
    public string ExternalId { get; init; }
}
