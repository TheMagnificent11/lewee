using FluentAssertions;
using Lewee.Auth.Domain;
using Xunit;

namespace Lewee.Auth.Domain.Tests.Unit;

public sealed class TenantTests
{
    [Fact]
    public void Should_RaiseTenantCreatedEvent_When_TenantCreated()
    {
        const string tenantCode = "TENANT";
        const string tenantName = "Tenant";
        var correlationId = Guid.NewGuid();

        var tenant = Tenant.Create(tenantCode, tenantName, correlationId);

        tenant.Id.Should().NotBeEmpty();
        tenant.Code.Should().Be(tenantCode);
        tenant.Name.Should().Be(tenantName);
        tenant.DomainEvents.GetAndClear().Should().ContainSingle()
            .Which.Should().BeEquivalentTo(
                new TenantCreatedEvent(tenant.Id, tenantCode, tenantName, correlationId));
    }
}
