using Lewee.StateManagement;
using Microsoft.Extensions.DependencyInjection;

namespace Pizzeria.Store.StateManagement;

public static class StoreStateManagementConfiguration
{
    public static IServiceCollection AddStoreState(this IServiceCollection services, bool isDevelopment)
    {
        services.AddLeweeFluxor(
            isDevelopment,
            typeof(StoreStateManagementConfiguration).Assembly);

        return services;
    }
}
