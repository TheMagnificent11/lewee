using Lewee.Application;
using Microsoft.Extensions.DependencyInjection;
using Sample.Pizzeria.Domain;

namespace Sample.Pizzeria.Application;

public static class PizzeriaApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddPizzeriaApplication(this IServiceCollection services)
    {
        services.AddApplication(typeof(UserOrderDto).Assembly, typeof(Order).Assembly);

        return services;
    }
}
