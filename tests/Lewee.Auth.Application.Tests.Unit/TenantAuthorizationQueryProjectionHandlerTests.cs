using FluentAssertions;
using Lewee.Auth.Domain;
using Lewee.Domain;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Lewee.Auth.Application.Tests.Unit;

public sealed class TenantAuthorizationQueryProjectionHandlerTests
{
    private static readonly Guid CorrelationId = Guid.NewGuid();

    [Fact]
    public async Task Should_RecordMembershipWithNoRoles_When_MembershipCreated()
    {
        const string externalUserId = "external-id";
        var tenantId = Guid.NewGuid();
        var user = User.Create(externalUserId, CorrelationId);
        user.AssignToTenant(tenantId, CorrelationId);

        var userRepository = CreateUserRepository(user);
        var roleRepository = new Mock<IRepository<Role>>();
        var queryProjectionService = new Mock<IQueryProjectionService>();
        var handler = CreateHandler(userRepository.Object, roleRepository.Object, queryProjectionService.Object);

        await handler.Handle(
            new TenantMembershipCreatedEvent(user.Id, tenantId, CorrelationId),
            CancellationToken.None);

        var expectedKey = TenantMembershipRolesQueryProjection.BuildKey(tenantId, externalUserId);
        queryProjectionService.Verify(
            item => item.AddOrUpdateAsync(
                It.Is<TenantMembershipRolesQueryProjection>(
                    projection => projection.IsMember && projection.RoleCodes.Count == 0),
                expectedKey,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_RecordNonMember_When_MembershipRemoved()
    {
        const string externalUserId = "external-id";
        var tenantId = Guid.NewGuid();
        var user = User.Create(externalUserId, CorrelationId);

        var userRepository = CreateUserRepository(user);
        var roleRepository = new Mock<IRepository<Role>>();
        var queryProjectionService = new Mock<IQueryProjectionService>();
        var handler = CreateHandler(userRepository.Object, roleRepository.Object, queryProjectionService.Object);

        await handler.Handle(
            new TenantMembershipRemovedEvent(user.Id, tenantId, CorrelationId),
            CancellationToken.None);

        var expectedKey = TenantMembershipRolesQueryProjection.BuildKey(tenantId, externalUserId);
        queryProjectionService.Verify(
            item => item.AddOrUpdateAsync(
                It.Is<TenantMembershipRolesQueryProjection>(
                    projection => !projection.IsMember && projection.RoleCodes.Count == 0),
                expectedKey,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_IncludeRoleCode_When_RoleAssigned()
    {
        const string externalUserId = "external-id";
        var tenantId = Guid.NewGuid();
        var role = Role.Create("EDITOR", "Editor", CorrelationId);
        var user = User.Create(externalUserId, CorrelationId);
        user.AssignToTenant(tenantId, CorrelationId);
        user.AssignRole(tenantId, role.Id, CorrelationId);

        var userRepository = CreateUserRepository(user);
        var roleRepository = new Mock<IRepository<Role>>();
        roleRepository
            .Setup(item => item.RetrieveByIdAsync(role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        var queryProjectionService = new Mock<IQueryProjectionService>();
        var handler = CreateHandler(userRepository.Object, roleRepository.Object, queryProjectionService.Object);

        await handler.Handle(
            new TenantRoleAssignedEvent(user.Id, tenantId, role.Id, CorrelationId),
            CancellationToken.None);

        var expectedKey = TenantMembershipRolesQueryProjection.BuildKey(tenantId, externalUserId);
        queryProjectionService.Verify(
            item => item.AddOrUpdateAsync(
                It.Is<TenantMembershipRolesQueryProjection>(
                    projection => projection.IsMember && projection.RoleCodes.SequenceEqual(new[] { role.Code })),
                expectedKey,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_ExcludeRoleCode_When_RoleRemoved()
    {
        const string externalUserId = "external-id";
        var tenantId = Guid.NewGuid();
        var user = User.Create(externalUserId, CorrelationId);
        user.AssignToTenant(tenantId, CorrelationId);

        var userRepository = CreateUserRepository(user);
        var roleRepository = new Mock<IRepository<Role>>();
        var queryProjectionService = new Mock<IQueryProjectionService>();
        var handler = CreateHandler(userRepository.Object, roleRepository.Object, queryProjectionService.Object);

        await handler.Handle(
            new TenantRoleRemovedEvent(user.Id, tenantId, Guid.NewGuid(), CorrelationId),
            CancellationToken.None);

        var expectedKey = TenantMembershipRolesQueryProjection.BuildKey(tenantId, externalUserId);
        queryProjectionService.Verify(
            item => item.AddOrUpdateAsync(
                It.Is<TenantMembershipRolesQueryProjection>(
                    projection => projection.IsMember && projection.RoleCodes.Count == 0),
                expectedKey,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_NotUpdateProjection_When_UserNotFound()
    {
        var userRepository = new Mock<IRepository<User>>();
        userRepository
            .Setup(item => item.RetrieveByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);
        var roleRepository = new Mock<IRepository<Role>>();
        var queryProjectionService = new Mock<IQueryProjectionService>();
        var handler = CreateHandler(userRepository.Object, roleRepository.Object, queryProjectionService.Object);

        await handler.Handle(
            new TenantMembershipCreatedEvent(Guid.NewGuid(), Guid.NewGuid(), CorrelationId),
            CancellationToken.None);

        queryProjectionService.Verify(
            item => item.AddOrUpdateAsync(
                It.IsAny<TenantMembershipRolesQueryProjection>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static Mock<IRepository<User>> CreateUserRepository(User user)
    {
        var userRepository = new Mock<IRepository<User>>();
        userRepository
            .Setup(item => item.RetrieveByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        return userRepository;
    }

    private static TenantAuthorizationQueryProjectionHandler CreateHandler(
        IRepository<User> userRepository,
        IRepository<Role> roleRepository,
        IQueryProjectionService queryProjectionService) =>
        new(
            userRepository,
            roleRepository,
            queryProjectionService,
            NullLogger<TenantAuthorizationQueryProjectionHandler>.Instance);
}
