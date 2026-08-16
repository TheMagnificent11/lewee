using FastEndpoints;
using Lewee.Auth.Api;
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

builder.Services
    .AddAuthenticatedUserService()
    .AddLeweePostgreSQL<AuthDbContext>(
        builder.Configuration.GetConnectionString(ServiceNames.PizzaStoreDatabaseName)!,
        typeof(User).Assembly,
        AuthDbContext.SchemaName)
    .AddLeweeDatabaseServices<AuthDbContext>(typeof(User).Assembly)
    .AddLeweePostgreSQL<StoreDbContext>(
        builder.Configuration.GetConnectionString(ServiceNames.PizzaStoreDatabaseName)!,
        typeof(Pizza).Assembly,
        StoreDbContext.SchemaName)
    .AddLeweeDatabaseServices<StoreDbContext>(typeof(Pizza).Assembly)
    .AddPizzaStoreApplication()
    .AddLeweeAuthApplication()
    .AddCorrelationIdServices()
    .AddKeycloakAuthenticationForWebApi(
        keycloakServiceName: ServiceNames.AuthServer,
        keycloakRealmName: CommonEnvironments.Auth.RealmName,
        keycloakClientId: CommonEnvironments.Auth.Clients.StoreApi,
        requireHttpsMetadata: false)
    .AddDatabaseHealthCheck<StoreDbContext>()
    .AddDatabaseHealthCheck<AuthDbContext>()
    .AddClientEventBroadcaster()
    .AddFastEndpoints(options =>
    {
        options.Assemblies = [typeof(Program).Assembly, typeof(CreateUserRequest).Assembly];
    });

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
