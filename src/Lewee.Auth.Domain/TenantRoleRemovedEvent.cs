using Lewee.Domain;

namespace Lewee.Auth.Domain;

/// <summary>
/// Raised when a role is removed from a user's membership of a tenant.
/// </summary>
public sealed class TenantRoleRemovedEvent : DomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TenantRoleRemovedEvent"/> class.
    /// </summary>
    /// <param name="userEntityId">User entity ID.</param>
    /// <param name="tenantEntityId">Tenant entity ID.</param>
    /// <param name="roleId">Role ID.</param>
    /// <param name="correlationId">Correlation ID.</param>
    public TenantRoleRemovedEvent(Guid userEntityId, Guid tenantEntityId, Guid roleId, Guid correlationId)
        : base(correlationId)
    {
        this.UserEntityId = userEntityId;
        this.TenantEntityId = tenantEntityId;
        this.RoleId = roleId;
    }

    /// <summary>
    /// Gets the user entity ID.
    /// </summary>
    public Guid UserEntityId { get; }

    /// <summary>
    /// Gets the tenant entity ID.
    /// </summary>
    public Guid TenantEntityId { get; }

    /// <summary>
    /// Gets the role ID.
    /// </summary>
    public Guid RoleId { get; }
}
