using FluentAssertions;
using Lewee.Auth.Domain;
using Lewee.Auth.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using Pizzeria.Auth;
using Pizzeria.Common;
using Xunit;

namespace Pizzeria.Configuration.Tests.Unit;

public sealed class AuthSeederTests
{
    private const string ExternalId = "external-admin-id";

    [Fact]
    public async Task Should_CreateUserAsSiteAdministratorWithoutTenant_When_UserDoesNotExistAsync()
    {
        using var dbContext = CreateDbContext(nameof(this.Should_CreateUserAsSiteAdministratorWithoutTenant_When_UserDoesNotExistAsync));
        var seeder = new AuthSeeder(dbContext, CreateAuthServerAdminClient());

        await seeder.RunAsync(CancellationToken.None);

        var user = await dbContext.Users.SingleAsync(item => item.ExternalId == ExternalId);
        user.IsSiteAdministrator.Should().BeTrue();
        user.TenantMemberships.Should().BeEmpty();
        dbContext.Tenants.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_MakeExistingUserSiteAdministrator_When_UserAlreadyExistsAsync()
    {
        using var dbContext = CreateDbContext(nameof(this.Should_MakeExistingUserSiteAdministrator_When_UserAlreadyExistsAsync));
        dbContext.Users.Add(User.Create(ExternalId, Guid.NewGuid()));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var seeder = new AuthSeeder(dbContext, CreateAuthServerAdminClient());

        await seeder.RunAsync(CancellationToken.None);

        var user = await dbContext.Users.SingleAsync(item => item.ExternalId == ExternalId);
        user.IsSiteAdministrator.Should().BeTrue();
    }

    private static AuthDbContext CreateDbContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new AuthDbContext(options);
    }

    private static IAuthServerAdminClient CreateAuthServerAdminClient()
    {
        var client = new Mock<IAuthServerAdminClient>();
        client
            .Setup(item => item.GetUserIdAsync(
                Environments.Auth.DefaultAdminCredentialsForTesting.Username,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ExternalId);

        return client.Object;
    }
}
