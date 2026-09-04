using FluentAssertions;
using Lewee.Auth.Domain;
using Xunit;

namespace Lewee.Auth.Domain.Tests.Unit;

public sealed class RoleTests
{
    [Fact]
    public void Should_RaiseRoleDefinedEvent_When_RoleCreated()
    {
        const string roleCode = "MANAGER";
        const string roleName = "Manager";
        var correlationId = Guid.NewGuid();

        var role = Role.Create(roleCode, roleName, correlationId);

        role.Id.Should().NotBeEmpty();
        role.Code.Should().Be(roleCode);
        role.Name.Should().Be(roleName);
        role.DomainEvents.GetAndClear().Should().ContainSingle()
            .Which.Should().BeEquivalentTo(
                new RoleDefinedEvent(role.Id, roleCode, roleName, correlationId));
    }
}
