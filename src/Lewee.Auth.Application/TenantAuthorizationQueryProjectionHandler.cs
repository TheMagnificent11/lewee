using System.Diagnostics.CodeAnalysis;
using Lewee.Auth.Domain;
using Lewee.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lewee.Auth.Application;

/// <summary>
/// Keeps the <see cref="TenantMembershipRolesQueryProjection"/> authorization lookup in sync with tenant
/// membership and role assignment changes.
/// </summary>
[SuppressMessage(
    "Performance",
    "CA1812:Avoid uninstantiated internal classes",
    Justification = "Registered via MediatR assembly scanning")]
internal sealed class TenantAuthorizationQueryProjectionHandler :
    INotificationHandler<TenantMembershipCreatedEvent>,
    INotificationHandler<TenantMembershipRemovedEvent>,
    INotificationHandler<TenantRoleAssignedEvent>,
    INotificationHandler<TenantRoleRemovedEvent>
{
    private readonly IRepository<User> userRepository;
    private readonly IRepository<Role> roleRepository;
    private readonly IQueryProjectionService queryProjectionService;
    private readonly ILogger<TenantAuthorizationQueryProjectionHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantAuthorizationQueryProjectionHandler"/> class.
    /// </summary>
    /// <param name="userRepository">User repository.</param>
    /// <param name="roleRepository">Role repository.</param>
    /// <param name="queryProjectionService">Query projection service.</param>
    /// <param name="logger">Logger.</param>
    public TenantAuthorizationQueryProjectionHandler(
        IRepository<User> userRepository,
        IRepository<Role> roleRepository,
        IQueryProjectionService queryProjectionService,
        ILogger<TenantAuthorizationQueryProjectionHandler> logger)
    {
        this.userRepository = userRepository;
        this.roleRepository = roleRepository;
        this.queryProjectionService = queryProjectionService;
        this.logger = logger;
    }

    /// <inheritdoc />
    public Task Handle(TenantMembershipCreatedEvent notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        return this.UpdateProjectionAsync(
            nameof(TenantMembershipCreatedEvent),
            notification.UserEntityId,
            notification.TenantEntityId,
            notification.CorrelationId,
            cancellationToken);
    }

    /// <inheritdoc />
    public Task Handle(TenantMembershipRemovedEvent notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        return this.UpdateProjectionAsync(
            nameof(TenantMembershipRemovedEvent),
            notification.UserEntityId,
            notification.TenantEntityId,
            notification.CorrelationId,
            cancellationToken);
    }

    /// <inheritdoc />
    public Task Handle(TenantRoleAssignedEvent notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        return this.UpdateProjectionAsync(
            nameof(TenantRoleAssignedEvent),
            notification.UserEntityId,
            notification.TenantEntityId,
            notification.CorrelationId,
            cancellationToken);
    }

    /// <inheritdoc />
    public Task Handle(TenantRoleRemovedEvent notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        return this.UpdateProjectionAsync(
            nameof(TenantRoleRemovedEvent),
            notification.UserEntityId,
            notification.TenantEntityId,
            notification.CorrelationId,
            cancellationToken);
    }

    private async Task UpdateProjectionAsync(
        string domainEventName,
        Guid userEntityId,
        Guid tenantEntityId,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        this.logger.LogHandlingDomainEvent(domainEventName, userEntityId, tenantEntityId);

        var user = await this.userRepository.RetrieveByIdAsync(userEntityId, cancellationToken);

        if (user == null)
        {
            this.logger.LogUserNotFound(userEntityId);
            return;
        }

        var membership = user.TenantMemberships.FirstOrDefault(x => x.TenantId == tenantEntityId);
        var key = TenantMembershipRolesQueryProjection.BuildKey(tenantEntityId, user.ExternalId);

        if (membership == null)
        {
            var removedProjection = new TenantMembershipRolesQueryProjection
            {
                CorrelationId = correlationId,
                IsMember = false,
            };

            await this.queryProjectionService.AddOrUpdateAsync(removedProjection, key, cancellationToken);
            this.logger.LogUpdatedProjection(userEntityId, tenantEntityId);

            return;
        }

        var roleCodes = new List<string>();

        foreach (var roleId in membership.RoleIds)
        {
            var role = await this.roleRepository.RetrieveByIdAsync(roleId, cancellationToken);

            if (role != null)
            {
                roleCodes.Add(role.Code);
            }
        }

        var projection = new TenantMembershipRolesQueryProjection
        {
            CorrelationId = correlationId,
            IsMember = true,
            RoleCodes = roleCodes,
        };

        await this.queryProjectionService.AddOrUpdateAsync(projection, key, cancellationToken);
        this.logger.LogUpdatedProjection(userEntityId, tenantEntityId);
    }
}
