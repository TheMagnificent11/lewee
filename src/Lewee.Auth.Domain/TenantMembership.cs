using System.Diagnostics.CodeAnalysis;
using Lewee.Domain;

namespace Lewee.Auth.Domain;

/// <summary>
/// A user's membership of a tenant.
/// </summary>
public sealed class TenantMembership : Entity
{
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
}
