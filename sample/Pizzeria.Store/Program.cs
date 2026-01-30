using Lewee.Infrastructure.Auth;
using Lewee.Infrastructure.Correlate;
using Lewee.Infrastructure.Data;
using Lewee.Infrastructure.Fluxor;
using Lewee.Infrastructure.Keycloak;
using Lewee.Infrastructure.PostgreSQL;
using Lewee.Infrastructure.ServerEvents;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using MudBlazor.Services;
using Pizzeria.Common;
using Pizzeria.ServiceDefaults;
using Pizzeria.Store;
using Pizzeria.Store.Application;
using Pizzeria.Store.Contracts;
using Pizzeria.Store.Data;
using Pizzeria.Store.Domain;
using Pizzeria.Store.Infrastructure;
using Pizzeria.Store.StateManagement;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Server services
builder.Services
    .AddAuthenticatedUserService()
    .AddLeweePostgreSQL<StoreDbContext>(
        builder.Configuration.GetConnectionString(ServiceNames.PizzaStoreDatabaseName)!,
        typeof(Pizza).Assembly,
        StoreDbContext.SchemaName)
    .AddLeweeDatabaseServices<StoreDbContext>(typeof(Pizza).Assembly)
    .AddPizzaStoreApplication()
    .AddCorrelationIdServices()
    .AddKeycloakAuthenticationForWebApp(
        keycloakServiceName: ServiceNames.AuthServer,
        keycloakRealmName: Pizzeria.Common.Environments.Auth.RealmName,
        keycloakClientId: Pizzeria.Common.Environments.Auth.Clients.StoreWeb,
        events: new OpenIdConnectEvents
        {
            OnTokenValidated = async context => await context.CreateCustomerOnFirstLoginAsync(),
        })
    .AddDatabaseHealthCheck<StoreDbContext>()
    .AddClientEventChannel();

// Client services
builder.Services
    .AddStoreState(builder.Environment.IsDevelopment())
    .AddSseMessageReceiver<MessageToActionMapper>()
    .AddMudServices()
    .AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

app.UseHealthEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(PageRoutes.Error, createScopeForErrors: true);
    app.UseHsts();
}

app
    .UseAntiforgery()
    .UseHttpsRedirection()
    .UseCorrelationIdMiddleware()
    .UseAuthentication()
    .UseAuthorization();

app.MapStaticAssets();

// Map SSE endpoint for real-time events
app.MapSseEndpoint();

// Map Keycloak sign-out
app.MapKeycloakSignOut(PageRoutes.SignOut);

// Map Blazor components
app
    .MapRazorComponents<App>()
    .AddAdditionalAssemblies(typeof(Pizzeria.Store.Components._Imports).Assembly)
    .AddInteractiveServerRenderMode();

await app.RunAsync();
