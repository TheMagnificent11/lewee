using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

    private sealed class LoggerWrapper<T> : ILogger<T>
    {
        private readonly ILogger innerLogger;

        public LoggerWrapper(ILogger innerLogger)
        {
            this.innerLogger = innerLogger;
        }

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
            => this.innerLogger.BeginScope(state);

        public bool IsEnabled(LogLevel logLevel)
            => this.innerLogger.IsEnabled(logLevel);

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            => this.innerLogger.Log(logLevel, eventId, state, exception, formatter);
    }
}
