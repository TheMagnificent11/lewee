namespace Lewee.Application.Mediation.Requests;

/// <summary>
/// Tenant Request Interface
/// </summary>
public interface ITenantRequest
{
    /// <summary>
    /// Gets the tenant ID
    /// </summary>
    Guid TenantId { get; }
}
