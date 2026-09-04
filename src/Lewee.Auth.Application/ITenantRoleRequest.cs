using Lewee.Application.Mediation.Requests;

namespace Lewee.Auth.Application;

/// <summary>
/// Marks a command or query as restricted to a caller holding at least one of a set of roles within the
/// tenant it pertains to.
/// </summary>
public interface ITenantRoleRequest : ITenantRequest
{
    /// <summary>
    /// Gets the set of role codes that satisfy this request - the caller need only hold one of them.
    /// </summary>
    IReadOnlyCollection<string> Roles { get; }
}
