using Lewee.Blazor;
using Pizzeria.Common;
using Refit;

namespace Pizzeria.Store.Web.Infrastructure;

internal static class ApiCientConfiguration
{
    public const string ApiClientName = "PizzeriaApi";

    public static IServiceCollection AddApiClient(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<AuthTokenDelegatingHandler>();

        services
            .AddRefitClient<IPizzeriaApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri($"https://{ServiceNames.PizzaStoreApi}"))
            .AddHttpMessageHandler<AuthTokenDelegatingHandler>()
            .AddCorrelationIdDelegationHandler();

        services.AddHttpClient(ApiClientName, c => c.BaseAddress = new Uri($"https://{ServiceNames.PizzaStoreApi}"));

        return services;
    }
}
