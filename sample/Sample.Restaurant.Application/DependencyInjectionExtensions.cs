using Lewee.Application;
using Microsoft.Extensions.DependencyInjection;
using Sample.Restaurant.Domain;

namespace Sample.Restaurant.Application;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddRestaurantApplication(this IServiceCollection services)
    {
        services.AddApplication(typeof(TableDto).Assembly, typeof(Table).Assembly);
        services.AddPipelineBehaviors(typeof(TableLoggingBehavior<,>));

        return services;
    }
}
