using FastEndpoints;
using Lewee.Auth.Application;
using Lewee.Auth.Domain;
using Lewee.Auth.Infrastructure.Data;
using Lewee.Infrastructure.Auth;
using Lewee.Infrastructure.Correlate;
using Lewee.Infrastructure.Data;
using Lewee.Infrastructure.Keycloak;
using Lewee.Infrastructure.PostgreSQL;
using Lewee.Infrastructure.ServerEvents;
using Pizzeria.Common;
using Pizzeria.ServiceDefaults;
using Pizzeria.Store.Application;
using Pizzeria.Store.Data;
using Pizzeria.Store.Domain;

using CommonEnvironments = Pizzeria.Common.Environments;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var pizzaStoreConnectionString =
    builder.Configuration.GetConnectionString(ServiceNames.PizzaStoreDatabaseName)!;

builder.Services
    .AddAuthenticatedUserService()
    .AddLeweePostgreSQL<StoreDbContext>(
        pizzaStoreConnectionString,
        typeof(Pizza).Assembly,
        StoreDbContext.SchemaName)
    .AddLeweeDatabaseServices<StoreDbContext>(typeof(Pizza).Assembly)
    .AddPizzaStoreApplication()

    // Register the auth database services last so that the tenant-role authorization pipeline
    // resolves the IQueryProjectionService bound to the auth schema, where the tenant membership
    // role projection is maintained.
    .AddLeweePostgreSQL<AuthDbContext>(
        pizzaStoreConnectionString,
        typeof(User).Assembly,
        AuthDbContext.SchemaName)
    .AddLeweeApplicationAuth()
    .AddCorrelationIdServices()
    .AddKeycloakAuthenticationForWebApi(
        keycloakServiceName: ServiceNames.AuthServer,
        keycloakRealmName: CommonEnvironments.Auth.RealmName,
        keycloakClientId: CommonEnvironments.Auth.Clients.StoreApi,
        requireHttpsMetadata: false)
    .AddDatabaseHealthCheck<StoreDbContext>()
    .AddClientEventBroadcaster()
    .AddFastEndpoints();

var app = builder.Build();

app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app
    .UseHttpsRedirection()
    .UseCorrelationIdMiddleware()
    .UseAuthentication()
    .UseAuthorization();

app.UseFastEndpoints();

app.MapSseEndpoint();

await app.RunAsync();
