using Lewee.Domain;

namespace Lewee.Auth.Application;

/// <summary>
/// A read model of the roles a user holds for a tenant, used by pipeline authorization behaviors to avoid a
/// per-request join across membership and role-assignment tables.
/// </summary>
public sealed class TenantMembershipRolesQueryProjection : IQueryProjection
{
    /// <inheritdoc />
    public Guid CorrelationId { get; init; }

    /// <summary>
    /// Gets a value indicating whether the user is currently a member of the tenant.
    /// </summary>
    public bool IsMember { get; init; }

    /// <summary>
    /// Gets the role codes currently held by the user's membership of the tenant.
    /// </summary>
    public IReadOnlyCollection<string> RoleCodes { get; init; } = [];

    /// <summary>
    /// Builds the query projection key for a tenant and a user's external identity.
    /// </summary>
    /// <param name="tenantId">Tenant ID.</param>
    /// <param name="externalUserId">User's external identity.</param>
    /// <returns>The query projection key.</returns>
    public static string BuildKey(Guid tenantId, string externalUserId) => $"{tenantId}:{externalUserId}";
}
