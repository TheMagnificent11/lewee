using Lewee.Application;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Pizzeria.Store.Application.Orders;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application;

public static class PizzaStoreApplicationConfiguration
{
    public static IServiceCollection AddPizzaStoreApplication(this IServiceCollection services)
    {
        services.AddApplication(
            typeof(PizzaStoreApplicationConfiguration).Assembly,
            typeof(Pizza).Assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(OrderOwnershipAuthorizationBehavior<,>));

        return services;
    }
}
