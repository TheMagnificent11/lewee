using Microsoft.EntityFrameworkCore;
using Saji.Domain;
using Saji.Infrastructure.Data;
using Saji.Infrastructure.Events;
using Sample.Shop.Domain.Entities;
using Sample.Shop.Infrastructure.Data.Configuration;

namespace Sample.Shop.Infrastructure.Data;

public sealed class ShopDbContext : BaseApplicationDbContext<ShopDbContext>
{
    public ShopDbContext(
        DbContextOptions<ShopDbContext> options,
        DomainEventDispatcher<ShopDbContext> domainEventDispatcher)
        : base(options, domainEventDispatcher)
    {
    }

#nullable disable

    public DbSet<Product> Products { get; set; }

    public DbSet<EnumEntity<OrderStatus>> OrderStatuses { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderLine> OrderLines { get; set; }

#nullable enable

    public override string Schema => "shp";

    protected override void ConfigureDatabaseModel(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new OrderStatusConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderLineConfiguration());
    }
}
