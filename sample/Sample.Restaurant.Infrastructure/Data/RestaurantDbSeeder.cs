using Lewee.Domain;
using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sample.Restaurant.Domain;

namespace Sample.Restaurant.Infrastructure.Data;
public class RestaurantDbSeeder : IDatabaseSeeder
{
    private readonly RestaurantDbContext context;

    public RestaurantDbSeeder(RestaurantDbContext context)
    {
        this.context = context;
    }

    public async Task Run()
    {
        var hasNewTables = await this.Seed(Domain.Table.DefaultData);
        var hasNewMenuItems = await this.Seed(MenuItem.DefaultData);

        if (!hasNewTables && !hasNewMenuItems)
        {
            return;
        }

        await this.context.SaveChangesAsync();
    }

    public async Task<bool> Seed<T>(T[] data)
        where T : BaseAggregateRoot
    {
        var ids = data.Select(x => x.Id)
            .ToList();

        var existingItems = await this.context
            .Set<T>()
            .Where(x => ids.Contains(x.Id))
            .ToListAsync();

        var existingIds = existingItems.Select(x => x.Id)
            .ToList();

        var newItems = data.Where(x => !existingIds.Contains(x.Id));

        if (!newItems.Any())
        {
            return false;
        }

        await this.context.Set<T>()
            .AddRangeAsync(newItems);

        return true;
    }
}
