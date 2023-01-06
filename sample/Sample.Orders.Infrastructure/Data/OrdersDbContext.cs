using Lewee.Infrastructure.Data;
using Lewee.Infrastructure.Events;
using Microsoft.EntityFrameworkCore;
using Sample.Orders.Domain;
using Sample.Orders.Infrastructure.Data.Configuration;

namespace Sample.Orders.Infrastructure.Data;

public class OrdersDbContext : BaseApplicationDbContext<OrdersDbContext>
{
    public OrdersDbContext(
        DbContextOptions<OrdersDbContext> options,
        DomainEventDispatcher<OrdersDbContext> domainEventDispatcher)
        : base(options, domainEventDispatcher)
    {
    }

    public DbSet<MenuItem>? MenuItems { get; set; }
    public DbSet<Order>? Orders { get; set; }

    public override string Schema => "ord";

    protected override void ConfigureDatabaseModel(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MenuItemConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
    }
}
