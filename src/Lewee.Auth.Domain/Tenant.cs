using System.Diagnostics.CodeAnalysis;
using Lewee.Domain;

namespace Lewee.Auth.Domain;

/// <summary>
/// An organization to which users can belong.
/// </summary>
public sealed class Tenant : AggregateRoot
{
    private Tenant(string code, string name, Guid correlationId)
    {
        this.Code = code;
        this.Name = name;
        this.DomainEvents.Raise(new TenantCreatedEvent(this.Id, code, name, correlationId));
    }

    private Tenant(Guid id, string code, string name, Guid correlationId)
        : base(id)
    {
        this.Code = code;
        this.Name = name;
        this.DomainEvents.Raise(new TenantCreatedEvent(this.Id, code, name, correlationId));
    }

    [ExcludeFromCodeCoverage(Justification = "Only used by EF")]
    private Tenant()
    {
    }

    /// <summary>
    /// Gets the tenant code.
    /// </summary>
    public string Code { get; private set; }

    /// <summary>
    /// Gets the tenant name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Creates a tenant.
    /// </summary>
    /// <param name="code">Tenant code.</param>
    /// <param name="name">Tenant name.</param>
    /// <param name="correlationId">Correlation ID.</param>
    /// <returns>The tenant.</returns>
    public static Tenant Create(string code, string name, Guid correlationId) => new(code, name, correlationId);

    /// <summary>
    /// Creates a tenant with a well-known identity.
    /// </summary>
    /// <param name="id">The well-known tenant ID.</param>
    /// <param name="code">Tenant code.</param>
    /// <param name="name">Tenant name.</param>
    /// <param name="correlationId">Correlation ID.</param>
    /// <returns>The tenant.</returns>
    /// <remarks>
    /// Use this overload when a fixed, deterministic tenant identity is required - for example, a single
    /// well-known tenant seeded by an application so that its commands and queries can reference the tenant ID
    /// as a compile-time constant.
    /// </remarks>
    public static Tenant Create(Guid id, string code, string name, Guid correlationId) =>
        new(id, code, name, correlationId);

    /// <summary>
    /// Tenant field lengths.
    /// </summary>
    public static class FieldLengths
    {
        /// <summary>
        /// Maximum tenant code length.
        /// </summary>
        public const int Code = 10;

        /// <summary>
        /// Maximum tenant name length.
        /// </summary>
        public const int Name = 200;
    }
}
