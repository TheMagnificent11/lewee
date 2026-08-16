using Lewee.Auth.Domain;
using Lewee.Auth.Infrastructure.Data;
using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Pizzeria.Auth;
using Pizzeria.Common;

namespace Pizzeria.Configuration;

internal sealed class AuthSeeder : IDatabaseSeeder<AuthDbContext>
{
    private const string AdministrativeTenantName = "Pizzeria Administration";
    private readonly AuthDbContext dbContext;
    private readonly IAuthServerAdminClient authServerAdminClient;

    public AuthSeeder(AuthDbContext dbContext, IAuthServerAdminClient authServerAdminClient)
    {
        this.dbContext = dbContext;
        this.authServerAdminClient = authServerAdminClient;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        await this.authServerAdminClient.CreateUserAsync(
            Environments.Auth.DefaultAdminCredentialsForTesting.Username,
            Environments.Auth.DefaultAdminCredentialsForTesting.Password,
            cancellationToken);
        var externalId = await this.authServerAdminClient.GetUserIdAsync(
            Environments.Auth.DefaultAdminCredentialsForTesting.Username,
            cancellationToken);

        var tenant = await this.dbContext.Tenants
            .SingleOrDefaultAsync(item => item.Name == AdministrativeTenantName, cancellationToken);
        if (tenant == null)
        {
            tenant = Tenant.Create(AdministrativeTenantName, Guid.NewGuid());
            this.dbContext.Tenants.Add(tenant);
        }

        var user = await this.dbContext.Users
            .Include(item => item.TenantMemberships)
            .SingleOrDefaultAsync(item => item.ExternalId == externalId, cancellationToken);
        if (user == null)
        {
            user = User.Create(externalId, Guid.NewGuid());
            this.dbContext.Users.Add(user);
        }

        user.AssignToTenant(tenant.Id, Guid.NewGuid());
        await this.dbContext.SaveChangesAsync(cancellationToken);
    }
}
