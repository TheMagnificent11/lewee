using System.Reflection;
using Correlate.DependencyInjection;
using Fluxor;
using Fluxor.Blazor.Web.ReduxDevTools;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.Fluxor;

/// <summary>
/// Fluxor Configuration
/// </summary>
public static class StateManagementConfiguration
{
    /// <summary>
    /// Configures Fluxor
    /// </summary>
    /// <param name="services">Services collection</param>
    /// <param name="useReduxDevTools">Whether to turn on the Redux Dev Tools</param>
    /// <param name="statemanagementAssemblies">Fluxor statement management assemblies</param>
    /// <returns>Updated services collection</returns>
    public static IServiceCollection AddLeweeFluxor(
        this IServiceCollection services,
        bool useReduxDevTools = false,
        params Assembly[] statemanagementAssemblies)
    {
        services.AddFluxor(options =>
        {
            options.ScanAssemblies(Assembly.GetEntryAssembly(), statemanagementAssemblies);

            if (useReduxDevTools)
            {
                options.UseReduxDevTools();
            }
        });

        services.AddCorrelate();

        return services;
    }

    /// <summary>
    /// Adds the SSE message receiver configuration for WebAssembly
    /// </summary>
    /// <typeparam name="TMapper">Message to action mapper type</typeparam>
    /// <param name="services">Services collection</param>
    /// <param name="configureHttpClient">Action to configure the HttpClient for SSE</param>
    /// <returns>Updated services collection</returns>
    public static IServiceCollection AddSseMessageReceiver<TMapper>(
        this IServiceCollection services,
        Action<HttpClient>? configureHttpClient = null)
        where TMapper : class, IMessageToActionMapper
    {
        services.AddScoped<IMessageToActionMapper, TMapper>();

        services.AddTransient<WasmAuthTokenDelegatingHandler>();

        services.AddHttpClient<SseClientMessageReceiver>(client =>
        {
            configureHttpClient?.Invoke(client);
        })
        .AddHttpMessageHandler<WasmAuthTokenDelegatingHandler>();

        return services;
    }

    /// <summary>
    /// Adds the <see cref="AccessTokenService"/> and <see cref="PersistentStateAccessTokenProvider"/>
    /// for WebAssembly clients to receive persisted access tokens from the server.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Call this method in the WebAssembly client's Program.cs. The server must also register
    /// <see cref="AccessTokenService"/> and call <c>RegisterPersistentService</c> to enable
    /// automatic state persistence.
    /// </para>
    /// <para>
    /// The server-side <see cref="AccessTokenService"/> should override the <c>AccessToken</c> property
    /// to retrieve the token from <c>HttpContext.GetTokenAsync("access_token")</c>.
    /// </para>
    /// <para>
    /// Services are registered as singletons in WebAssembly because scoped and singleton lifetimes
    /// are equivalent in the browser context (single user per app instance), and the persistent
    /// state restoration occurs during app initialization before scopes are established.
    /// </para>
    /// <para>
    /// This method also registers <see cref="WasmAuthenticatedUserService"/> as the implementation
    /// of <see cref="Common.IAuthenticatedUserService"/> for WebAssembly clients.
    /// </para>
    /// </remarks>
    /// <param name="services">Services collection</param>
    /// <returns>Updated services collection</returns>
    public static IServiceCollection AddPersistentStateAccessToken(this IServiceCollection services)
    {
        // Use singleton in WebAssembly - scoped and singleton are equivalent in browser context,
        // and persistent state restoration happens before DI scopes are established
        services.AddSingleton<AccessTokenService>();
        services.AddSingleton<IAccessTokenProvider, PersistentStateAccessTokenProvider>();
        services.AddSingleton<Common.IAuthenticatedUserService, WasmAuthenticatedUserService>();

        return services;
    }
}
