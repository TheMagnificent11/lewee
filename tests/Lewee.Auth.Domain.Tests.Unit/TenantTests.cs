using FluentAssertions;
using Lewee.Auth.Domain;
using Xunit;

namespace Lewee.Auth.Domain.Tests.Unit;

public sealed class TenantTests
{
    [Fact]
    public void Create_Should_Raise_TenantCreatedEvent()
    {
        var correlationId = Guid.NewGuid();

        var tenant = Tenant.Create("Tenant", correlationId);

        tenant.Id.Should().NotBeEmpty();
        tenant.Name.Should().Be("Tenant");
        tenant.DomainEvents.GetAndClear().Should().ContainSingle()
            .Which.Should().BeEquivalentTo(
                new TenantCreatedEvent(tenant.Id, tenant.Name, correlationId));
    }
}
