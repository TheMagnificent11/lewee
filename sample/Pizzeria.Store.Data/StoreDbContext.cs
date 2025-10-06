using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Data;

public sealed class StoreDbContext : ApplicationDbContext<StoreDbContext>
{
    public const string SchemaName = "sto";

    public StoreDbContext(DbContextOptions<StoreDbContext> options)
        : base(options)
    {
    }

    public override string Schema => SchemaName;

    public DbSet<Pizza> Pizzas { get; set; }

    public DbSet<Order> Orders { get; set; }
}
