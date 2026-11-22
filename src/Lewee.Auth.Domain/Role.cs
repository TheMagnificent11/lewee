using System.Diagnostics.CodeAnalysis;
using Lewee.Domain;

namespace Lewee.Auth.Domain;

/// <summary>
/// A role that can be assigned to a user's membership of a tenant.
/// </summary>
/// <remarks>
/// Roles are defined once, globally, by a site administrator - not owned by, or defined per, <see cref="Tenant"/>.
/// Any tenant may assign any defined role to its own members.
/// </remarks>
public sealed class Role : AggregateRoot
{
    private Role(string code, string name, Guid correlationId)
    {
        this.Code = code;
        this.Name = name;
        this.DomainEvents.Raise(new RoleDefinedEvent(this.Id, code, name, correlationId));
    }

    [ExcludeFromCodeCoverage(Justification = "Only used by EF")]
    private Role()
    {
    }

    /// <summary>
    /// Gets the globally-unique role code.
    /// </summary>
    public string Code { get; private set; }

    /// <summary>
    /// Gets the role name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Defines a role.
    /// </summary>
    /// <param name="code">Globally-unique role code.</param>
    /// <param name="name">Role name.</param>
    /// <param name="correlationId">Correlation ID.</param>
    /// <returns>The role.</returns>
    public static Role Create(string code, string name, Guid correlationId) => new(code, name, correlationId);

    /// <summary>
    /// Role field lengths.
    /// </summary>
    public static class FieldLengths
    {
        /// <summary>
        /// Maximum role code length.
        /// </summary>
        public const int Code = 50;

        /// <summary>
        /// Maximum role name length.
        /// </summary>
        public const int Name = 200;
    }
}
