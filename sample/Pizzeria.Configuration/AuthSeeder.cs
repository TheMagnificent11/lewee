using Lewee.Auth.Domain;
using Lewee.Auth.Infrastructure.Data;
using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Pizzeria.Auth;
using Pizzeria.Common;

namespace Pizzeria.Configuration;

internal sealed class AuthSeeder : IDatabaseSeeder<AuthDbContext>
{
    private readonly AuthDbContext dbContext;
    private readonly IAuthServerAdminClient authServerAdminClient;

    public AuthSeeder(AuthDbContext dbContext, IAuthServerAdminClient authServerAdminClient)
    {
        this.dbContext = dbContext;
        this.authServerAdminClient = authServerAdminClient;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        await this.SeedSiteAdministratorAsync(cancellationToken);

        var tenant = await this.SeedTenantAsync(cancellationToken);
        var storeManagerRole = await this.SeedRoleAsync(
            PizzaStore.Roles.StoreManagerCode,
            PizzaStore.Roles.StoreManagerName,
            cancellationToken);
        var storeStaffRole = await this.SeedRoleAsync(
            PizzaStore.Roles.StoreStaffCode,
            PizzaStore.Roles.StoreStaffName,
            cancellationToken);

        await this.SeedTenantMemberAsync(
            Environments.Auth.DefaultStoreManagerCredentialsForTesting.Username,
            Environments.Auth.DefaultStoreManagerCredentialsForTesting.Password,
            tenant.Id,
            storeManagerRole.Id,
            cancellationToken);

        await this.SeedTenantMemberAsync(
            Environments.Auth.DefaultStoreStaffCredentialsForTesting.Username,
            Environments.Auth.DefaultStoreStaffCredentialsForTesting.Password,
            tenant.Id,
            storeStaffRole.Id,
            cancellationToken);

        await this.dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedSiteAdministratorAsync(CancellationToken cancellationToken)
    {
        await this.authServerAdminClient.CreateUserAsync(
            Environments.Auth.DefaultAdminCredentialsForTesting.Username,
            Environments.Auth.DefaultAdminCredentialsForTesting.Password,
            cancellationToken);
        var externalId = await this.authServerAdminClient.GetUserIdAsync(
            Environments.Auth.DefaultAdminCredentialsForTesting.Username,
            cancellationToken);

        var user = await this.dbContext.Users
            .SingleOrDefaultAsync(item => item.ExternalId == externalId, cancellationToken);
        if (user == null)
        {
            user = User.Create(externalId, Guid.NewGuid());
            this.dbContext.Users.Add(user);
        }

        // IsSiteAdministrator is set directly rather than via a domain method, given how rarely it changes.
        user.IsSiteAdministrator = true;

        await this.dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<Tenant> SeedTenantAsync(CancellationToken cancellationToken)
    {
        var tenant = await this.dbContext.Tenants
            .SingleOrDefaultAsync(x => x.Id == PizzaStore.Tenant.Id, cancellationToken);
        if (tenant == null)
        {
            tenant = Tenant.Create(
                PizzaStore.Tenant.Id,
                PizzaStore.Tenant.Code,
                PizzaStore.Tenant.Name,
                Guid.NewGuid());
            this.dbContext.Tenants.Add(tenant);
        }

        return tenant;
    }

    private async Task<Role> SeedRoleAsync(string code, string name, CancellationToken cancellationToken)
    {
        var role = await this.dbContext.Roles
            .SingleOrDefaultAsync(x => x.Code == code, cancellationToken);
        if (role == null)
        {
            role = Role.Create(code, name, Guid.NewGuid());
            this.dbContext.Roles.Add(role);
        }

        return role;
    }

    private async Task SeedTenantMemberAsync(
        string username,
        string password,
        Guid tenantId,
        Guid roleId,
        CancellationToken cancellationToken)
    {
        await this.authServerAdminClient.CreateUserAsync(username, password, cancellationToken);
        var externalId = await this.authServerAdminClient.GetUserIdAsync(username, cancellationToken);

        var user = await this.dbContext.Users
            .SingleOrDefaultAsync(item => item.ExternalId == externalId, cancellationToken);
        if (user == null)
        {
            user = User.Create(externalId, Guid.NewGuid());
            this.dbContext.Users.Add(user);
        }

        user.AssignToTenant(tenantId, Guid.NewGuid());
        user.AssignRole(tenantId, roleId, Guid.NewGuid());
    }
}
