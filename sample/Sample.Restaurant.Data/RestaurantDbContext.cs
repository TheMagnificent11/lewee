using Lewee.Domain;
using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sample.Restaurant.Data.Configuration;
using Sample.Restaurant.Domain;

namespace Sample.Restaurant.Data;

public abstract class RestaurantDbContext<TContext> : ApplicationDbContext<TContext>
    where TContext : DbContext, IApplicationDbContext
{
    protected RestaurantDbContext(
        DbContextOptions<TContext> options,
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
