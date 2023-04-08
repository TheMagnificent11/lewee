using Lewee.Domain;
using Lewee.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Sample.Restaurant.Domain;

namespace Sample.Restaurant.Infrastructure.Data;

public static class RestaurantDataConfiguration
{
    // TODO ((https://github.com/TheMagnificent11/lewee/issues/16)): remove this when generic registration is done
    public static IServiceCollection ConfigureRestaurantData(this IServiceCollection services)
    {
        services.AddTransient<IRepository<Table>, Repository<Table, RestaurantDbContext>>();
        services.AddTransient<IRepository<MenuItem>, Repository<MenuItem, RestaurantDbContext>>();

        return services;
    }
}
