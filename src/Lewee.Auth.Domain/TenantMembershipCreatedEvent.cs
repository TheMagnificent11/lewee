using Lewee.Domain;

namespace Lewee.Auth.Domain;

/// <summary>
/// Raised when a user is assigned to a tenant.
/// </summary>
public sealed class TenantMembershipCreatedEvent : DomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TenantMembershipCreatedEvent"/> class.
    /// </summary>
    /// <param name="userEntityId">User entity ID.</param>
    /// <param name="tenantEntityId">Tenant entity ID.</param>
    /// <param name="correlationId">Correlation ID.</param>
    public TenantMembershipCreatedEvent(Guid userEntityId, Guid tenantEntityId, Guid correlationId)
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
