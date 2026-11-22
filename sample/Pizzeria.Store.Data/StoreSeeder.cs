using System.Diagnostics.CodeAnalysis;
using Lewee.Infrastructure.Data;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Data;

public sealed class StoreSeeder : IDatabaseSeeder<StoreDbContext>
{
    private readonly StoreDbContext dbContext;

    public StoreSeeder(StoreDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [SuppressMessage(
        "Reliability",
        "CA2016:Forward the 'CancellationToken' parameter to methods",
        Justification = "False positive")]
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        await this.dbContext.Database.EnsureCreatedAsync(cancellationToken);

        var pizzas = Menu.Pizzas;
        var hasChanges = false;

        foreach (var item in pizzas)
        {
            var existing = await this.dbContext.Pizzas.FindAsync(item.Id, cancellationToken);

            if (existing == null)
            {
                this.dbContext.Pizzas.Add(item);
                hasChanges = true;
            }
        }

        if (!hasChanges)
        {
            return;
        }

        await this.dbContext.SaveChangesAsync(cancellationToken);
    }
}
