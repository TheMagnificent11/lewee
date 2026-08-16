using FluentAssertions;
using Lewee.Auth.Domain;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Lewee.Auth.Infrastructure.Data.Tests.Unit;

public sealed class AuthDbContextTests
{
    [Fact]
    public void Model_Should_Use_Auth_Schema_And_Unique_Indexes()
    {
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(nameof(AuthDbContextTests.Model_Should_Use_Auth_Schema_And_Unique_Indexes))
            .Options;
        using var context = new AuthDbContext(options);
        var userEntity = context.Model.FindEntityType(typeof(User));
        var membershipEntity = context.Model.FindEntityType(typeof(TenantMembership));

        context.Schema.Should().Be("auth");
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
