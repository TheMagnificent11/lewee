using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Pizzeria.Auth;

/// <summary>
/// Extension methods for adding auth server client services
/// </summary>
public static class AuthServerServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Keycloak auth server client to the service collection
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configureClient">Action to configure the HTTP client</param>
    /// <returns>HTTP client builder for further configuration</returns>
    public static IHttpClientBuilder AddAuthServerClient(
        this IServiceCollection services,
        Action<IServiceProvider, HttpClient> configureClient)
    {
        return services.AddHttpClient<IAuthServerClient, KeycloakHttpClient>(configureClient);
    }

    /// <summary>
    /// Creates an auth server client instance for testing purposes
    /// </summary>
    /// <param name="httpClient">HTTP client to use</param>
    /// <param name="logger">Logger instance</param>
    /// <returns>Auth server client instance</returns>
    public static IAuthServerClient CreateAuthServerClient(HttpClient httpClient, ILogger logger)
    {
        // Cast logger to the specific type needed by KeycloakHttpClient
        var typedLogger = logger as ILogger<KeycloakHttpClient>
            ?? new LoggerWrapper<KeycloakHttpClient>(logger);
        return new KeycloakHttpClient(httpClient, typedLogger);
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
