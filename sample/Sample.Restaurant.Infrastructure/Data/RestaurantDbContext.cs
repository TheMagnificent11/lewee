using Lewee.Domain;
using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sample.Restaurant.Domain;
using Sample.Restaurant.Infrastructure.Data.Configuration;

namespace Sample.Restaurant.Infrastructure.Data;

public class RestaurantDbContext : ApplicationDbContext<RestaurantDbContext>
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
        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
    }
}
