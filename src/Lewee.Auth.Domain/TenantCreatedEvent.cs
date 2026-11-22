using Lewee.Domain;

namespace Lewee.Auth.Domain;

/// <summary>
/// Raised when a tenant is created.
/// </summary>
public sealed class TenantCreatedEvent : DomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TenantCreatedEvent"/> class.
    /// </summary>
    /// <param name="tenantEntityId">Tenant entity ID.</param>
    /// <param name="code">Tenant code.</param>
    /// <param name="name">Tenant name.</param>
    /// <param name="correlationId">Correlation ID.</param>
    public TenantCreatedEvent(Guid tenantEntityId, string code, string name, Guid correlationId)
        : base(correlationId)
    {
        this.TenantEntityId = tenantEntityId;
        this.Code = code;
        this.Name = name;
    }

    /// <summary>
    /// Gets the tenant entity ID.
    /// </summary>
    public Guid TenantEntityId { get; }

    /// <summary>
    /// Gets the tenant code.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the tenant name.
    /// </summary>
    public string Name { get; }
}
