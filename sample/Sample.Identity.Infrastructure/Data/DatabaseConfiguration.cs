using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.Identity.Infrastructure.Data;

public static class DatabaseConfiguration
{
    public static void ConfigureIdentityDatabase(this IServiceCollection services, string connectionString)
    {
        services
            .AddDbContext<IdentityDbContext>(options => options.UseSqlServer(connectionString));

        services
            .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<IdentityDbContext>();
    }
}
