using Lewee.Common;
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
    public static IServiceCollection AddKeycloakAuthentication(
        this IServiceCollection services,
        string keycloakServiceName,
        string keycloakRealmName,
        string keycloakClientId,
        OpenIdConnectEvents? events = null)
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
                    options.RequireHttpsMetadata = false; // For development/testing only
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
