using System.Diagnostics.CodeAnalysis;
using Lewee.Domain;

namespace Lewee.Auth.Domain;

/// <summary>
/// An organization to which users can belong.
/// </summary>
public sealed class Tenant : AggregateRoot
{
    private Tenant(string name, Guid correlationId)
    {
        this.Name = name;
        this.DomainEvents.Raise(new TenantCreatedEvent(this.Id, name, correlationId));
    }

    [ExcludeFromCodeCoverage(Justification = "Only used by EF")]
    private Tenant()
    {
    }

    /// <summary>
    /// Gets the tenant name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Creates a tenant.
    /// </summary>
    /// <param name="name">Tenant name.</param>
    /// <param name="correlationId">Correlation ID.</param>
    /// <returns>The tenant.</returns>
    public static Tenant Create(string name, Guid correlationId) => new(name, correlationId);

    /// <summary>
    /// Tenant field lengths.
    /// </summary>
    public static class FieldLengths
    {
        /// <summary>
        /// Maximum tenant name length.
        /// </summary>
        public const int Name = 200;
    }
}
