using Lewee.Domain;
using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sample.Pizzeria.Domain;
using Sample.Pizzeria.Infrastructure.Data.Configuration;

namespace Sample.Pizzeria.Infrastructure.Data;

public sealed class PizzeriaDbContext : ApplicationDbContext<PizzeriaDbContext>
{
    public PizzeriaDbContext(
        DbContextOptions<PizzeriaDbContext> options,
        IAuthenticatedUserService authenticatedUserService)
        : base(options, authenticatedUserService)
    {
    }

    public DbSet<Order>? Orders { get; set; }

    protected override void ConfigureDatabaseModel(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderStatusConfiguration());
        modelBuilder.ApplyConfiguration(new OrderPizzaConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
    }
}
