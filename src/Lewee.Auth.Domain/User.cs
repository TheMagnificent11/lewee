using System.Diagnostics.CodeAnalysis;
using Lewee.Domain;

namespace Lewee.Auth.Domain;

/// <summary>
/// A user identified by an external identity provider.
/// </summary>
public sealed class User : AggregateRoot
{
    private readonly List<TenantMembership> tenantMemberships = [];

    private User(string externalId, Guid correlationId)
    {
        this.ExternalId = externalId;
        this.DomainEvents.Raise(new UserCreatedEvent(this.Id, externalId, correlationId));
    }

    [ExcludeFromCodeCoverage(Justification = "Only used by EF")]
    private User()
    {
    }

    /// <summary>
    /// Gets the external identity.
    /// </summary>
    public string ExternalId { get; private set; }

    /// <summary>
    /// Gets the tenant memberships.
    /// </summary>
    public IReadOnlyCollection<TenantMembership> TenantMemberships => this.tenantMemberships.AsReadOnly();

    /// <summary>
    /// Gets a value indicating whether the user is a site administrator, irrespective of tenant or role.
    /// </summary>
    /// <remarks>
    /// There is no public domain method to set this flag - given how rarely it changes, it is expected to be
    /// set directly, e.g. by <c>sample/Pizzeria.Configuration/AuthSeeder.cs</c>, rather than via a command.
    /// The setter is <see langword="internal"/> (not <see langword="private"/>) and exposed via
    /// <see cref="System.Runtime.CompilerServices.InternalsVisibleToAttribute"/> to the handful of trusted
    /// callers (e.g. a seeder) and unit test projects that need to set it directly.
    /// </remarks>
    public bool IsSiteAdministrator { get; internal set; }

    /// <summary>
    /// Creates a user.
    /// </summary>
    /// <param name="externalId">External identity.</param>
    /// <param name="correlationId">Correlation ID.</param>
    /// <returns>The user.</returns>
    public static User Create(string externalId, Guid correlationId) => new(externalId, correlationId);

    /// <summary>
    /// Assigns the user to a tenant.
    /// </summary>
    /// <param name="tenantId">Tenant ID.</param>
    /// <param name="correlationId">Correlation ID.</param>
    public void AssignToTenant(Guid tenantId, Guid correlationId)
    {
        if (this.tenantMemberships.Exists(x => x.TenantId == tenantId))
        {
            return;
        }

        this.tenantMemberships.Add(new TenantMembership(tenantId));
        this.DomainEvents.Raise(new TenantMembershipCreatedEvent(this.Id, tenantId, correlationId));
    }

    /// <summary>
    /// Removes the user from a tenant.
    /// </summary>
    /// <param name="tenantId">Tenant ID.</param>
    /// <param name="correlationId">Correlation ID.</param>
    public void RemoveFromTenant(Guid tenantId, Guid correlationId)
    {
        var membership = this.tenantMemberships.Find(x => x.TenantId == tenantId);
        if (membership == null)
        {
            return;
        }

        this.tenantMemberships.Remove(membership);
        this.DomainEvents.Raise(new TenantMembershipRemovedEvent(this.Id, tenantId, correlationId));
    }

    /// <summary>
    /// Assigns a role to the user's membership of a tenant.
    /// </summary>
    /// <param name="tenantId">Tenant ID.</param>
    /// <param name="roleId">Role ID.</param>
    /// <param name="correlationId">Correlation ID.</param>
    public void AssignRole(Guid tenantId, Guid roleId, Guid correlationId)
    {
        var membership = this.tenantMemberships.Find(x => x.TenantId == tenantId);
        if (membership == null)
        {
            return;
        }

        if (membership.AssignRole(roleId))
        {
            this.DomainEvents.Raise(new TenantRoleAssignedEvent(this.Id, tenantId, roleId, correlationId));
        }
    }

    /// <summary>
    /// Removes a role from the user's membership of a tenant.
    /// </summary>
    /// <param name="tenantId">Tenant ID.</param>
    /// <param name="roleId">Role ID.</param>
    /// <param name="correlationId">Correlation ID.</param>
    public void RemoveRole(Guid tenantId, Guid roleId, Guid correlationId)
    {
        var membership = this.tenantMemberships.Find(x => x.TenantId == tenantId);
        if (membership == null)
        {
            return;
        }

        if (membership.RemoveRole(roleId))
        {
            this.DomainEvents.Raise(new TenantRoleRemovedEvent(this.Id, tenantId, roleId, correlationId));
        }
    }

    /// <summary>
    /// User field lengths.
    /// </summary>
    public static class FieldLengths
    {
        /// <summary>
        /// Maximum external identity length.
        /// </summary>
        public const int ExternalId = 100;
    }
}
