using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.Keycloak;

/// <summary>
/// Keycloak Authentication Configuration
/// </summary>
public static class KeycloakAuthenticationConfiguration
{
    /// <summary>
    /// Adds Keycloak authentication using OpenID Connect
    /// </summary>
    /// <param name="services">Services collection</param>
    /// <param name="keycloakServiceName">Name of the Keycloak service for service discovery</param>
    /// <param name="keycloakRealmName">Keycloak realm name</param>
    /// <param name="keycloakClientId">Keycloak client ID</param>
    /// <param name="events">OpenID Connect events to handle authentication lifecycle</param>
    /// <returns>The updated services collection</returns>
    [Obsolete("Use AddKeycloakAuthenticationForWebApp instead")]
    [SuppressMessage(
        "SonarLint",
        "S1133:Deprecated code should be removed",
        Justification = "Keeping for backward compatibility during migration period")]
    public static IServiceCollection AddKeycloakAuthentication(
        this IServiceCollection services,
        string keycloakServiceName,
        string keycloakRealmName,
        string keycloakClientId,
        OpenIdConnectEvents? events = null)
    {
        return services.AddKeycloakAuthenticationForWebApp(
            keycloakServiceName,
            keycloakRealmName,
            keycloakClientId,
            events);
    }

    /// <summary>
    /// Adds Keycloak authentication for web applications using OpenID Connect
    /// </summary>
    /// <param name="services">Services collection</param>
    /// <param name="keycloakServiceName">Name of the Keycloak service for service discovery</param>
    /// <param name="keycloakRealmName">Keycloak realm name</param>
    /// <param name="keycloakClientId">Keycloak client ID</param>
    /// <param name="events">OpenID Connect events to handle authentication lifecycle</param>
    /// <param name="requireHttpsMetadata">
    /// Whether HTTPS is required for the metadata endpoint. Defaults to <see langword="true"/>.
    /// Set to <see langword="false"/> only for local development or testing environments.
    /// </param>
    /// <returns>The updated services collection</returns>
    public static IServiceCollection AddKeycloakAuthenticationForWebApp(
        this IServiceCollection services,
        string keycloakServiceName,
        string keycloakRealmName,
        string keycloakClientId,
        OpenIdConnectEvents? events = null,
        bool requireHttpsMetadata = true)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddKeycloakOpenIdConnect(
                keycloakServiceName,
                realm: keycloakRealmName,
                options =>
                {
                    options.ClientId = keycloakClientId;
                    options.ResponseType = "code"; // Use authorization code flow
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.RequireHttpsMetadata = requireHttpsMetadata;
                    options.UsePkce = true; // Enable PKCE for public clients

                    // Map claims
                    options.TokenValidationParameters.NameClaimType = "preferred_username";
                    options.TokenValidationParameters.RoleClaimType = "roles";

                    // Add scopes
                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");

                    // Apply custom events if provided
                    if (events is not null)
                    {
                        options.Events = events;
                    }
                });

        services.AddAuthorization();
        services.AddCascadingAuthenticationState();

        return services;
    }

    /// <summary>
    /// Adds Keycloak authentication for Web APIs using JWT Bearer tokens
    /// </summary>
    /// <param name="services">Services collection</param>
    /// <param name="keycloakServiceName">Name of the Keycloak service for service discovery</param>
    /// <param name="keycloakRealmName">Keycloak realm name</param>
    /// <param name="keycloakClientId">Keycloak client ID (used as valid audience)</param>
    /// <param name="requireHttpsMetadata">
    /// Whether HTTPS is required for the metadata endpoint. Defaults to <see langword="true"/>.
    /// Set to <see langword="false"/> only for local development or testing environments.
    /// </param>
    /// <returns>The updated services collection</returns>
    public static IServiceCollection AddKeycloakAuthenticationForWebApi(
        this IServiceCollection services,
        string keycloakServiceName,
        string keycloakRealmName,
        string keycloakClientId,
        bool requireHttpsMetadata = true)
    {
        services
            .AddAuthentication()
            .AddKeycloakJwtBearer(
                keycloakServiceName,
                realm: keycloakRealmName,
                options =>
                {
                    options.RequireHttpsMetadata = requireHttpsMetadata;

                    // Map claims
                    options.TokenValidationParameters.NameClaimType = "preferred_username";
                    options.TokenValidationParameters.RoleClaimType = "roles";
                    options.TokenValidationParameters.ValidAudience = keycloakClientId;
                });

        services.AddAuthorization();

        return services;
    }

    /// <summary>
    /// Maps the sign-out endpoint
    /// </summary>
    /// <param name="app">Web application</param>
    /// <param name="signOutRoute">Route pattern for the sign-out endpoint</param>
    /// <returns>The updated web application</returns>
    public static WebApplication MapKeycloakSignOut(this WebApplication app, string signOutRoute)
    {
        app
            .MapPost(signOutRoute, () =>
            {
                return Results.SignOut(
                    authenticationSchemes:
                    [
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        OpenIdConnectDefaults.AuthenticationScheme,
                    ]);
            })
            .RequireAuthorization();

        return app;
    }
}
