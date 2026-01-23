using System.Security.Claims;
using Lewee.Infrastructure.Keycloak;
using MediatR;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Pizzeria.Common;
using Pizzeria.Store.Application.Customers;
using Pizzeria.Store.Contracts;

namespace Pizzeria.Store.Web.Infrastructure;

internal static class AuthConfiguration
{
    public static IServiceCollection AddAuth(this IServiceCollection services)
    {
        services.AddKeycloakAuthentication(
            keycloakServiceName: ServiceNames.AuthServer,
            keycloakRealmName: Pizzeria.Common.Environments.Auth.RealmName,
            keycloakClientId: Pizzeria.Common.Environments.Auth.Clients.StoreWeb,
            events: new OpenIdConnectEvents
            {
                OnTokenValidated = async context =>
                {
                    var externalUserId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (!string.IsNullOrEmpty(externalUserId))
                    {
                        var mediator = context.HttpContext.RequestServices.GetRequiredService<IMediator>();
                        var createCustomerCommand = new CreateCustomerCommand(
                            externalUserId,
                            CorrelationId: Guid.NewGuid());

                        try
                        {
                            _ = await mediator.Send(createCustomerCommand, context.HttpContext.RequestAborted);
                        }
                        catch
                        {
                            // Ignore errors - user might already exist or API might be temporarily unavailable
                            // This shouldn't prevent the user from accessing the application
                        }
                    }
                },
            });

        return services;
    }

    public static WebApplication MapSignOut(this WebApplication app)
    {
        app.MapKeycloakSignOut(PageRoutes.SignOut);

        return app;
    }
}
