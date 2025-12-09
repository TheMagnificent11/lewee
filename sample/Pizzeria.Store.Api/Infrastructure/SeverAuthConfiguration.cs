using Microsoft.AspNetCore.Authentication.JwtBearer;
using Pizzeria.Common;

namespace Pizzeria.Store.Api.Infrastructure;

internal static class SeverAuthConfiguration
{
    public static IServiceCollection AddAuth(this IServiceCollection services, bool isDevOrTest)
    {
        services
            .AddAuthentication()
            .AddKeycloakJwtBearer(
                serviceName: ServiceNames.AuthServer,
                realm: Pizzeria.Common.Environments.Auth.RealmName,
                options =>
                {
                    // Disable HTTPS metadata requirement for local/containerized Keycloak
                    options.RequireHttpsMetadata = !isDevOrTest;

                    // Configure token validation parameters
                    options.TokenValidationParameters.ValidateAudience = true;
                    options.TokenValidationParameters.ValidAudiences = [
                        Pizzeria.Common.Environments.Auth.Clients.StoreApi,
                        "account",
                    ];

                    // Add events to log authentication failures for debugging
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                                .CreateLogger("JwtBearerAuthentication");
                            logger.LogError(context.Exception, "JWT Authentication failed");
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                                .CreateLogger("JwtBearerAuthentication");
                            logger.LogDebug("JWT Token validated successfully for user: {User}", context.Principal?.Identity?.Name);
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                                .CreateLogger("JwtBearerAuthentication");
                            logger.LogWarning(
                                "JWT Challenge issued. Error: {Error}, ErrorDescription: {ErrorDescription}",
                                context.Error,
                                context.ErrorDescription);
                            return Task.CompletedTask;
                        },
                    };
                });

        services.AddAuthorizationBuilder();

        return services;
    }
}
