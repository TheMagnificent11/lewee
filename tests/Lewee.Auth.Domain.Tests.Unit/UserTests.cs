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
}
