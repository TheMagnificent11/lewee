using Lewee.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.Restaurant.Application;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddRestaurantApplication(this IServiceCollection services)
    {
        services.AddApplication(typeof(IRestaurantDbContext).Assembly);
        services.AddPipelineBehaviors(typeof(TableLoggingBehavior<,>));

        return services;
    }
}
