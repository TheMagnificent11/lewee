using Lewee.Application;
using Microsoft.Extensions.DependencyInjection;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application;

public static class PizzaStoreApplicationConfiguration
{
    public static IServiceCollection AddPizzaStoreApplication(this IServiceCollection services)
    {
        services.AddApplication(
            typeof(PizzaStoreApplicationConfiguration).Assembly,
            typeof(Pizza).Assembly);

        return services;
    }
}
