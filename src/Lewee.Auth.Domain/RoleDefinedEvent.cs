using Lewee.Domain;

namespace Lewee.Auth.Domain;

/// <summary>
/// Raised when a role is defined.
/// </summary>
public sealed class RoleDefinedEvent : DomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RoleDefinedEvent"/> class.
    /// </summary>
    /// <param name="roleId">Role ID.</param>
    /// <param name="code">Role code.</param>
    /// <param name="name">Role name.</param>
    /// <param name="correlationId">Correlation ID.</param>
    public RoleDefinedEvent(Guid roleId, string code, string name, Guid correlationId)
        : base(correlationId)
    {
        this.RoleId = roleId;
        this.Code = code;
        this.Name = name;
    }

    /// <summary>
    /// Gets the role ID.
    /// </summary>
    public Guid RoleId { get; }

    /// <summary>
    /// Gets the role code.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the role name.
    /// </summary>
    public string Name { get; }
}
