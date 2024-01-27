using Lewee.Domain;
using Microsoft.EntityFrameworkCore;
using Sample.Restaurant.Data;

namespace Sample.Restaurant.SqlServer;

public sealed class SqlServerRestaurantDbContext : RestaurantDbContext<SqlServerRestaurantDbContext>
{
    public SqlServerRestaurantDbContext(
        DbContextOptions<SqlServerRestaurantDbContext> options,
        IAuthenticatedUserService authenticatedUserService)
        : base(options, authenticatedUserService)
    {
    }
}
