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
        var roleEntity = context.Model.FindEntityType(typeof(Role));

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
        var isSiteAdministratorProperty = userEntity.FindProperty(nameof(User.IsSiteAdministrator));
        isSiteAdministratorProperty.Should().NotBeNull();
        isSiteAdministratorProperty.IsNullable.Should().BeFalse();
        membershipEntity.Should().NotBeNull();
        membershipEntity.GetIndexes()
            .Should().Contain(index => index.IsUnique &&
                index.Properties.Select(property => property.Name)
                    .SequenceEqual(new[] { "UserId", nameof(TenantMembership.TenantId) }));
        membershipEntity.FindProperty(nameof(TenantMembership.RoleIds)).Should().NotBeNull();
        roleEntity.Should().NotBeNull();
        var roleCodeProperty = roleEntity.FindProperty(nameof(Role.Code));
        roleCodeProperty.Should().NotBeNull();
        roleCodeProperty.GetMaxLength().Should().Be(Role.FieldLengths.Code);
        roleEntity.GetIndexes()
            .Should().Contain(index => index.IsUnique &&
                index.Properties.Select(property => property.Name).SequenceEqual(new[] { nameof(Role.Code) }));
        var roleNameProperty = roleEntity.FindProperty(nameof(Role.Name));
        roleNameProperty.Should().NotBeNull();
        roleNameProperty.GetMaxLength().Should().Be(Role.FieldLengths.Name);
    }
}
