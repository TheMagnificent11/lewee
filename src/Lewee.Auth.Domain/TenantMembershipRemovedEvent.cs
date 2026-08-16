using Lewee.Domain;

namespace Lewee.Auth.Domain;

/// <summary>
/// Raised when a user is removed from a tenant.
/// </summary>
public sealed class TenantMembershipRemovedEvent : DomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TenantMembershipRemovedEvent"/> class.
    /// </summary>
    /// <param name="userEntityId">User entity ID.</param>
    /// <param name="tenantEntityId">Tenant entity ID.</param>
    /// <param name="correlationId">Correlation ID.</param>
    public TenantMembershipRemovedEvent(Guid userEntityId, Guid tenantEntityId, Guid correlationId)
        : base(correlationId)
    {
        this.UserEntityId = userEntityId;
        this.TenantEntityId = tenantEntityId;
    }

    /// <summary>
    /// Gets the user entity ID.
    /// </summary>
    public Guid UserEntityId { get; }

    /// <summary>
    /// Gets the tenant entity ID.
    /// </summary>
    public Guid TenantEntityId { get; }
}
