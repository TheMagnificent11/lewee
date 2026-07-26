using System.Diagnostics.CodeAnalysis;
using Lewee.Domain;

namespace Pizzeria.Store.Domain;

public class User : AggregateRoot
{
    internal User(string externalId, Guid correlationId)
        : base()
    {
        this.ExternalId = externalId;

        this.DomainEvents.Raise(new UserCreatedEvent(
            this.Id,
            this.ExternalId,
            correlationId));
    }

    [ExcludeFromCodeCoverage(Justification = "Only used by EF")]
    private User()
        : base()
    {
    }

    public string ExternalId { get; protected set; }

    public static User Create(string externalId, Guid correlationId)
    {
        return new User(externalId, correlationId);
    }

    public static class FieldLengths
    {
        public const int ExternalId = 100;
    }
}
