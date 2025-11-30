using Lewee.Blazor;
using Pizzeria.Common;
using Refit;

namespace Pizzeria.Store.Web.Infrastructure;

internal static class ApiCientConfiguration
{
    public const string ApiClientName = "PizzeriaApi";

    public static IServiceCollection AddApiClient(this IServiceCollection services)
    {
        services
            .AddRefitClient<IPizzeriaApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri($"https://{ServiceNames.PizzaStoreApi}"))
            .AddCorrelationIdDelegationHandler();

        services.AddHttpClient(ApiClientName, c => c.BaseAddress = new Uri($"https://{ServiceNames.PizzaStoreApi}"));

        return services;
    }
}
