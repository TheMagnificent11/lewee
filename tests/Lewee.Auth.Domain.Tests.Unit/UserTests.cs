using System.Linq;
using FluentAssertions;
using Lewee.Auth.Domain;
using Xunit;

namespace Lewee.Auth.Domain.Tests.Unit;

public sealed class UserTests
{
    [Fact]
    public void Should_CreateUserWithoutMemberships_When_UserCreated()
    {
        const string externalUserId = "external-id";
        var user = User.Create(externalUserId, Guid.NewGuid());

        user.ExternalId.Should().Be(externalUserId);
        user.TenantMemberships.Should().BeEmpty();
        user.IsSiteAdministrator.Should().BeFalse();
        user.DomainEvents.GetAndClear().Should().ContainSingle()
            .Which.Should().BeOfType<UserCreatedEvent>();
    }

    [Fact]
    public void Should_AddMembershipsAndRaiseEvents_When_UserAssignedToTenants()
    {
        const string externalUserId = "external-id";
        var user = User.Create(externalUserId, Guid.NewGuid());
        var firstTenantId = Guid.NewGuid();
        var secondTenantId = Guid.NewGuid();
        user.DomainEvents.GetAndClear();

        user.AssignToTenant(firstTenantId, Guid.NewGuid());
        user.AssignToTenant(firstTenantId, Guid.NewGuid());
        user.AssignToTenant(secondTenantId, Guid.NewGuid());

        user.TenantMemberships.Select(membership => membership.TenantId)
            .Should().BeEquivalentTo([firstTenantId, secondTenantId]);
        user.DomainEvents.GetAndClear().Should().HaveCount(2)
            .And.AllBeOfType<TenantMembershipCreatedEvent>();
    }

    [Fact]
    public void Should_RemoveMembershipAndRaiseSingleEvent_When_UserRemovedFromTenantTwice()
    {
        const string externalUserId = "external-id";
        var user = User.Create(externalUserId, Guid.NewGuid());
        var tenantId = Guid.NewGuid();
        user.AssignToTenant(tenantId, Guid.NewGuid());
        user.DomainEvents.GetAndClear();

        user.RemoveFromTenant(tenantId, Guid.NewGuid());
        user.RemoveFromTenant(tenantId, Guid.NewGuid());

        user.TenantMemberships.Should().BeEmpty();
        user.DomainEvents.GetAndClear().Should().ContainSingle()
            .Which.Should().BeOfType<TenantMembershipRemovedEvent>();
    }

    [Fact]
    public void Should_HaveNoRoles_When_TenantMembershipCreated()
    {
        var user = User.Create("external-id", Guid.NewGuid());
        var tenantId = Guid.NewGuid();

        user.AssignToTenant(tenantId, Guid.NewGuid());

        user.TenantMemberships.Single(membership => membership.TenantId == tenantId)
            .RoleIds.Should().BeEmpty();
    }

    [Fact]
    public void Should_AssignRoleAndRaiseEvent_When_RoleNotAlreadyHeld()
    {
        var user = User.Create("external-id", Guid.NewGuid());
        var tenantId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        user.AssignToTenant(tenantId, Guid.NewGuid());
        user.DomainEvents.GetAndClear();

        user.AssignRole(tenantId, roleId, Guid.NewGuid());

        user.TenantMemberships.Single(membership => membership.TenantId == tenantId)
            .RoleIds.Should().ContainSingle().Which.Should().Be(roleId);
        user.DomainEvents.GetAndClear().Should().ContainSingle()
            .Which.Should().BeOfType<TenantMembershipRoleAssignedEvent>();
    }

    [Fact]
    public void Should_NotDuplicateRoleOrRaiseEvent_When_RoleAssignedTwice()
    {
        var user = User.Create("external-id", Guid.NewGuid());
        var tenantId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        user.AssignToTenant(tenantId, Guid.NewGuid());
        user.AssignRole(tenantId, roleId, Guid.NewGuid());
        user.DomainEvents.GetAndClear();

        user.AssignRole(tenantId, roleId, Guid.NewGuid());

        user.TenantMemberships.Single(membership => membership.TenantId == tenantId)
            .RoleIds.Should().ContainSingle();
        user.DomainEvents.GetAndClear().Should().BeEmpty();
    }

    [Fact]
    public void Should_HoldMultipleRoles_When_MultipleRolesAssignedToSameMembership()
    {
        var user = User.Create("external-id", Guid.NewGuid());
        var tenantId = Guid.NewGuid();
        var firstRoleId = Guid.NewGuid();
        var secondRoleId = Guid.NewGuid();
        user.AssignToTenant(tenantId, Guid.NewGuid());

        user.AssignRole(tenantId, firstRoleId, Guid.NewGuid());
        user.AssignRole(tenantId, secondRoleId, Guid.NewGuid());

        user.TenantMemberships.Single(membership => membership.TenantId == tenantId)
            .RoleIds.Should().BeEquivalentTo([firstRoleId, secondRoleId]);
    }

    [Fact]
    public void Should_RemoveRoleAndRaiseEvent_When_RoleHeld()
    {
        var user = User.Create("external-id", Guid.NewGuid());
        var tenantId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        user.AssignToTenant(tenantId, Guid.NewGuid());
        user.AssignRole(tenantId, roleId, Guid.NewGuid());
        user.DomainEvents.GetAndClear();

        user.RemoveRole(tenantId, roleId, Guid.NewGuid());

        user.TenantMemberships.Single(membership => membership.TenantId == tenantId)
            .RoleIds.Should().BeEmpty();
        user.DomainEvents.GetAndClear().Should().ContainSingle()
            .Which.Should().BeOfType<TenantMembershipRoleRemovedEvent>();
    }

    [Fact]
    public void Should_NotRaiseEvent_When_RemovingRoleNotHeld()
    {
        var user = User.Create("external-id", Guid.NewGuid());
        var tenantId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        user.AssignToTenant(tenantId, Guid.NewGuid());
        user.DomainEvents.GetAndClear();

        user.RemoveRole(tenantId, roleId, Guid.NewGuid());

        user.TenantMemberships.Single(membership => membership.TenantId == tenantId)
            .RoleIds.Should().BeEmpty();
        user.DomainEvents.GetAndClear().Should().BeEmpty();
    }

    [Fact]
    public void Should_RemoveRoleAssignments_When_TenantMembershipRemoved()
    {
        var user = User.Create("external-id", Guid.NewGuid());
        var tenantId = Guid.NewGuid();
        user.AssignToTenant(tenantId, Guid.NewGuid());
        user.AssignRole(tenantId, Guid.NewGuid(), Guid.NewGuid());

        user.RemoveFromTenant(tenantId, Guid.NewGuid());
        user.AssignToTenant(tenantId, Guid.NewGuid());

        user.TenantMemberships.Single(membership => membership.TenantId == tenantId)
            .RoleIds.Should().BeEmpty();
    }
}
