using FluentAssertions;
using Lewee.Auth.Domain;
using Xunit;

namespace Lewee.Auth.Domain.Tests.Unit;

public sealed class UserTests
{
    [Fact]
    public void Create_Should_Create_User_Without_Memberships()
    {
        var user = User.Create("external-id", Guid.NewGuid());

        user.ExternalId.Should().Be("external-id");
        user.TenantMemberships.Should().BeEmpty();
        user.DomainEvents.GetAndClear().Should().ContainSingle()
            .Which.Should().BeOfType<UserCreatedEvent>();
    }

    [Fact]
    public void AssignToTenant_Should_Add_Memberships_And_Raise_Events()
    {
        var user = User.Create("external-id", Guid.NewGuid());
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
    public void RemoveFromTenant_Should_Be_Idempotent_And_Raise_Event()
    {
        var user = User.Create("external-id", Guid.NewGuid());
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
