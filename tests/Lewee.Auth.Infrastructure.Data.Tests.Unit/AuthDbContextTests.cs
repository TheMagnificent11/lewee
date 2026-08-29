using FluentAssertions;
using Lewee.Auth.Domain;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Lewee.Auth.Infrastructure.Data.Tests.Unit;

public sealed class AuthDbContextTests
{
    [Fact]
    public void Should_UseAuthSchemaAndUniqueIndexes_When_ModelCreated()
    {
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(nameof(AuthDbContextTests.Should_UseAuthSchemaAndUniqueIndexes_When_ModelCreated))
            .Options;
        using var context = new AuthDbContext(options);
        var tenantEntity = context.Model.FindEntityType(typeof(Tenant));
        var userEntity = context.Model.FindEntityType(typeof(User));
        var membershipEntity = context.Model.FindEntityType(typeof(TenantMembership));

        context.Schema.Should().Be("auth");
        tenantEntity.Should().NotBeNull();
        var tenantCodeProperty = tenantEntity.FindProperty(nameof(Tenant.Code));
        tenantCodeProperty.Should().NotBeNull();
        tenantCodeProperty.GetMaxLength().Should().Be(Tenant.FieldLengths.Code);
        tenantEntity.GetIndexes()
            .Should().Contain(index => index.IsUnique &&
                index.Properties.Select(property => property.Name).SequenceEqual(new[] { nameof(Tenant.Code) }));
        userEntity.Should().NotBeNull();
        userEntity.GetIndexes()
            .Should().Contain(index => index.IsUnique &&
                index.Properties.Select(property => property.Name).SequenceEqual(new[] { nameof(User.ExternalId) }));
        membershipEntity.Should().NotBeNull();
        membershipEntity.GetIndexes()
            .Should().Contain(index => index.IsUnique &&
                index.Properties.Select(property => property.Name)
                    .SequenceEqual(new[] { "UserId", nameof(TenantMembership.TenantId) }));
    }
}
