using Microsoft.Extensions.DependencyInjection;

namespace Pizzeria.Auth;

public static class AuthServerServiceCollectionExtensions
{
    public static IHttpClientBuilder AddKeycloakAdminClient(
        this IServiceCollection services,
        string realmName,
        Action<HttpClient> configureClient)
    {
        services.AddMemoryCache();

        services.Configure<KeycloakOptions>(options =>
        {
            options.RealmName = realmName;
        });

        return services
            .AddTransient<KeycloakAdminTokenHandler>()
            .AddHttpClient<IAuthServerAdminClient, KeycloakHttpClient>(configureClient)
            .AddHttpMessageHandler<KeycloakAdminTokenHandler>();
    }
}
