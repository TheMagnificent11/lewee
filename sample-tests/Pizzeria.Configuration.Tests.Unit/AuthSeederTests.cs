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
    private const string AdminExternalId = "external-admin-id";
    private const string StoreManagerExternalId = "external-store-manager-id";
    private const string StoreStaffExternalId = "external-store-staff-id";

    [Fact]
    public async Task Should_CreateUserAsSiteAdministratorWithoutTenant_When_UserDoesNotExistAsync()
    {
        using var dbContext = CreateDbContext(nameof(this.Should_CreateUserAsSiteAdministratorWithoutTenant_When_UserDoesNotExistAsync));
        var seeder = new AuthSeeder(dbContext, CreateAuthServerAdminClient());

        await seeder.RunAsync(CancellationToken.None);

        var user = await dbContext.Users.SingleAsync(item => item.ExternalId == AdminExternalId);
        user.IsSiteAdministrator.Should().BeTrue();
        user.TenantMemberships.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_MakeExistingUserSiteAdministrator_When_UserAlreadyExistsAsync()
    {
        using var dbContext = CreateDbContext(nameof(this.Should_MakeExistingUserSiteAdministrator_When_UserAlreadyExistsAsync));
        dbContext.Users.Add(User.Create(AdminExternalId, Guid.NewGuid()));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var seeder = new AuthSeeder(dbContext, CreateAuthServerAdminClient());

        await seeder.RunAsync(CancellationToken.None);

        var user = await dbContext.Users.SingleAsync(item => item.ExternalId == AdminExternalId);
        user.IsSiteAdministrator.Should().BeTrue();
    }

    [Fact]
    public async Task Should_SeedThePizzeriaTenant_When_SeedingAsync()
    {
        using var dbContext = CreateDbContext(nameof(this.Should_SeedThePizzeriaTenant_When_SeedingAsync));
        var seeder = new AuthSeeder(dbContext, CreateAuthServerAdminClient());

        await seeder.RunAsync(CancellationToken.None);

        var tenant = await dbContext.Tenants.SingleAsync(x => x.Id == PizzaStore.Tenant.Id);
        tenant.Code.Should().Be(PizzaStore.Tenant.Code);
        tenant.Name.Should().Be(PizzaStore.Tenant.Name);
    }

    [Fact]
    public async Task Should_SeedStoreManagerAndStoreStaffRolesWithDistinctCodes_When_SeedingAsync()
    {
        using var dbContext = CreateDbContext(nameof(this.Should_SeedStoreManagerAndStoreStaffRolesWithDistinctCodes_When_SeedingAsync));
        var seeder = new AuthSeeder(dbContext, CreateAuthServerAdminClient());

        await seeder.RunAsync(CancellationToken.None);

        var roles = await dbContext.Roles.ToListAsync();
        roles.Select(x => x.Code).Should().Contain(
            [PizzaStore.Roles.StoreManagerCode, PizzaStore.Roles.StoreStaffCode]);
        roles.Select(x => x.Code).Should().OnlyHaveUniqueItems();
        PizzaStore.Roles.StoreManagerCode.Should().NotBe(PizzaStore.Roles.StoreStaffCode);
    }

    [Fact]
    public async Task Should_SeedTenantMembershipForEachRole_When_SeedingAsync()
    {
        using var dbContext = CreateDbContext(nameof(this.Should_SeedTenantMembershipForEachRole_When_SeedingAsync));
        var seeder = new AuthSeeder(dbContext, CreateAuthServerAdminClient());

        await seeder.RunAsync(CancellationToken.None);

        var storeManagerRole = await dbContext.Roles.SingleAsync(x => x.Code == PizzaStore.Roles.StoreManagerCode);
        var storeStaffRole = await dbContext.Roles.SingleAsync(x => x.Code == PizzaStore.Roles.StoreStaffCode);

        var manager = await dbContext.Users
            .Include(x => x.TenantMemberships)
            .SingleAsync(x => x.ExternalId == StoreManagerExternalId);
        var managerMembership = manager.TenantMemberships.Single(x => x.TenantId == PizzaStore.Tenant.Id);
        managerMembership.RoleIds.Should().Contain(storeManagerRole.Id);

        var staff = await dbContext.Users
            .Include(x => x.TenantMemberships)
            .SingleAsync(x => x.ExternalId == StoreStaffExternalId);
        var staffMembership = staff.TenantMemberships.Single(x => x.TenantId == PizzaStore.Tenant.Id);
        staffMembership.RoleIds.Should().Contain(storeStaffRole.Id);
    }

    [Fact]
    public async Task Should_BeIdempotent_When_SeedingRunsTwiceAsync()
    {
        using var dbContext = CreateDbContext(nameof(this.Should_BeIdempotent_When_SeedingRunsTwiceAsync));
        var seeder = new AuthSeeder(dbContext, CreateAuthServerAdminClient());

        await seeder.RunAsync(CancellationToken.None);
        await seeder.RunAsync(CancellationToken.None);

        dbContext.Tenants.Should().ContainSingle();
        (await dbContext.Roles.CountAsync()).Should().Be(2);
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
            .ReturnsAsync(AdminExternalId);

        client
            .Setup(item => item.GetUserIdAsync(
                Environments.Auth.DefaultStoreManagerCredentialsForTesting.Username,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(StoreManagerExternalId);

        client
            .Setup(item => item.GetUserIdAsync(
                Environments.Auth.DefaultStoreStaffCredentialsForTesting.Username,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(StoreStaffExternalId);

        return client.Object;
    }
}
