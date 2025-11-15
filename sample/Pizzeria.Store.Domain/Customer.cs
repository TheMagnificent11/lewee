using System.Diagnostics.CodeAnalysis;
using Lewee.Domain;

namespace Pizzeria.Store.Domain;

public class Customer : AggregateRoot
{
    internal Customer(string externalId, Guid correlationId)
        : base()
    {
        this.ExternalId = externalId;

        this.DomainEvents.Raise(new CustomerCreatedEvent(
            this.Id,
            this.ExternalId,
            correlationId));
    }

    [ExcludeFromCodeCoverage(Justification = "Only used by EF")]
    private Customer()
        : base()
    {
    }

    public string ExternalId { get; protected set; }

    public static Customer Create(string externalId, Guid correlationId)
    {
        return new Customer(externalId, correlationId);
    }

    public static class FieldLengths
    {
        public const int ExternalId = 100;
    }
}
