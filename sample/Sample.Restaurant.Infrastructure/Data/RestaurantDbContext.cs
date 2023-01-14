using Lewee.Application.Auth;
using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sample.Restaurant.Application;
using Sample.Restaurant.Domain;
using Sample.Restaurant.Infrastructure.Data.Configuration;

namespace Sample.Restaurant.Infrastructure.Data;

public class RestaurantDbContext : BaseApplicationDbContext<RestaurantDbContext>, IRestaurantDbContext
{
    public RestaurantDbContext(
        DbContextOptions<RestaurantDbContext> options,
        IAuthenticatedUserService authenticatedUserService)
        : base(options, authenticatedUserService)
    {
    }

    public override string Schema => "res";

    public DbSet<Table>? Tables { get; set; }
    public DbSet<MenuItem>? MenuItems { get; set; }

    protected override void ConfigureDatabaseModel(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TableConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderStatusConfiguration());
        modelBuilder.ApplyConfiguration(new MenuItemConfiguration());
        modelBuilder.ApplyConfiguration(new MenuItemTypeConfiguration());
    }
}
