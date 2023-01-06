﻿using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sample.Orders.Application;
using Sample.Orders.Domain;
using Sample.Orders.Infrastructure.Data.Configuration;

namespace Sample.Orders.Infrastructure.Data;

public class OrdersDbContext : BaseApplicationDbContext<OrdersDbContext>, IOrdersDbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options)
        : base(options)
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
