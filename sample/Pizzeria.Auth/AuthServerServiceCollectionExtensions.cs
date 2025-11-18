using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Pizzeria.Auth;

public static class AuthServerServiceCollectionExtensions
{
    public static IHttpClientBuilder AddAuthServerAdminClient(
        this IServiceCollection services,
        Action<IServiceProvider, HttpClient> configureClient)
    {
        // Ensure memory cache is registered
        services.AddMemoryCache();

        // Register the HTTP client with the delegating handler
        return services
            .AddTransient<KeycloakAdminTokenHandler>()
            .AddHttpClient<IAuthServerAdminClient, KeycloakHttpClient>(configureClient)
            .AddHttpMessageHandler<KeycloakAdminTokenHandler>();
    }

    public static IAuthServerAdminClient CreateAuthServerAdminClient(
        HttpClient httpClient,
        ILogger logger,
        IMemoryCache memoryCache)
    {
        // Create a delegating handler-wrapped HTTP client
        var tokenHandlerLogger = new LoggerWrapper<KeycloakAdminTokenHandler>(logger);
        var tokenHandler = new KeycloakAdminTokenHandler(memoryCache, tokenHandlerLogger)
        {
            InnerHandler = new HttpClientHandler(),
        };

        var wrappedClient = new HttpClient(tokenHandler)
        {
            BaseAddress = httpClient.BaseAddress,
            Timeout = httpClient.Timeout,
        };

        // Copy headers
        foreach (var header in httpClient.DefaultRequestHeaders)
        {
            wrappedClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
        }

        var typedLogger = logger as ILogger<KeycloakHttpClient>
            ?? new LoggerWrapper<KeycloakHttpClient>(logger);
        return new KeycloakHttpClient(wrappedClient, typedLogger);
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
