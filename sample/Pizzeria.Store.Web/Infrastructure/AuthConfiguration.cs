using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Pizzeria.Common;
using Pizzeria.Store.Contracts;

namespace Pizzeria.Store.Web.Infrastructure;

internal static class AuthConfiguration
{
    public static IServiceCollection AddAuth(this IServiceCollection services)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "OpenIdConnect";
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddKeycloakOpenIdConnect(
                ServiceNames.AuthServer,
                realm: Pizzeria.Common.Environments.Auth.RealmName,
                options =>
                {
                    options.ClientId = Pizzeria.Common.Environments.Auth.Clients.StoreWeb;
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

                    // Handle user creation on first login
                    options.Events = new OpenIdConnectEvents
                    {
                        OnTokenValidated = async context =>
                        {
                            var externalUserId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                            if (!string.IsNullOrEmpty(externalUserId))
                            {
                                var apiClient = context.HttpContext.RequestServices.GetRequiredService<IPizzeriaApiClient>();
                                try
                                {
                                    // Create user entity in database if it doesn't exist
                                    await apiClient.CreateCustomerAsync(
                                        new CreateCustomerRequest
                                        {
                                            ExternalUserId = externalUserId,
                                        },
                                        context.HttpContext.RequestAborted);
                                }
                                catch
                                {
                                    // Ignore errors - user might already exist or API might be temporarily unavailable
                                    // This shouldn't prevent the user from accessing the application
                                }
                            }
                        },
                    };
                });

        services.AddAuthorization();
        services.AddCascadingAuthenticationState();

        return services;
    }

    public static WebApplication MapSignOut(this WebApplication app)
    {
        app
            .MapPost(PageRoutes.SignOut, () =>
            {
                return TypedResults.SignOut(
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
