using System.Diagnostics.CodeAnalysis;
using Lewee.Domain;

namespace Lewee.Auth.Domain;

/// <summary>
/// A user's membership of a tenant.
/// </summary>
public sealed class TenantMembership : Entity
{
    private readonly List<Guid> roleIds = [];

    internal TenantMembership(Guid tenantId)
    {
        this.TenantId = tenantId;
    }

    [ExcludeFromCodeCoverage(Justification = "Only used by EF")]
    private TenantMembership()
    {
    }

    /// <summary>
    /// Gets the tenant ID.
    /// </summary>
    public Guid TenantId { get; private set; }

    /// <summary>
    /// Gets the IDs of the roles assigned to this membership.
    /// </summary>
    public IReadOnlyCollection<Guid> RoleIds => this.roleIds.AsReadOnly();

    /// <summary>
    /// Assigns a role to this membership.
    /// </summary>
    /// <param name="roleId">Role ID.</param>
    /// <returns><see langword="true"/> if the role was newly assigned, otherwise <see langword="false"/> if it was already held.</returns>
    internal bool AssignRole(Guid roleId)
    {
        if (this.roleIds.Contains(roleId))
        {
            return false;
        }

        this.roleIds.Add(roleId);
        return true;
    }

    /// <summary>
    /// Removes a role from this membership.
    /// </summary>
    /// <param name="roleId">Role ID.</param>
    /// <returns><see langword="true"/> if the role was held and removed, otherwise <see langword="false"/>.</returns>
    internal bool RemoveRole(Guid roleId)
    {
        return this.roleIds.Remove(roleId);
    }
}
