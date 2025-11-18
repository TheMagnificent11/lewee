using FastEndpoints;
using Lewee.Infrastructure.AspNet.Auth;
using Lewee.Infrastructure.AspNet.Observability;
using Lewee.Infrastructure.AspNet.SignalR;
using Lewee.Infrastructure.Data;
using Lewee.Infrastructure.PostgreSQL;
using Pizzeria.Auth;
using Pizzeria.Common;
using Pizzeria.ServiceDefaults;
using Pizzeria.Store.Api.Startup;
using Pizzeria.Store.Application;
using Pizzeria.Store.Data;
using Pizzeria.Store.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var databaseName = ServiceNames.GetPizzaStoreDatabaseName();
var isDevOrTest = builder.Environment.IsDevelopment() || Pizzeria.Common.Environments.IsIntegrationTesting;

builder.Services
    .AddAuthenticatedUserService()
    .AddLeweePostgreSQL<StoreDbContext>(
        builder.Configuration.GetConnectionString(databaseName)!,
        typeof(Pizza).Assembly,
        StoreDbContext.SchemaName)
    .AddLeweeDatabaseServices<StoreDbContext>(typeof(Pizza).Assembly)
    .AddPizzaStoreApplication()
    .AddCorrelationIdServices()
    .AddLeweeSignalR();

// Register auth server client with service discovery
builder.Services.AddAuthServerClient((serviceProvider, client) =>
{
    // Use service discovery to get the Keycloak URL
    client.BaseAddress = new Uri($"http://{ServiceNames.AuthServer}");
    client.Timeout = TimeSpan.FromMinutes(1);
});

builder.Services
    .AddAuthentication()
    .AddKeycloakJwtBearer(
        serviceName: ServiceNames.AuthServer,
        realm: Pizzeria.Common.Environments.Auth.RealmName,
        options =>
        {
            // Disable HTTPS metadata requirement for local/containerized Keycloak
            options.RequireHttpsMetadata = !isDevOrTest;

            // TODO: Integration tests are currently failing with 401 Unauthorized
            // This appears to be an issue with JWT validation in the Aspire testing environment
            // Possible causes:
            // 1. Service discovery URL mismatch between token issuer and API authority
            // 2. JWKS endpoint not reachable from API container
            // 3. Timing issue with Keycloak readiness
        });

builder.Services.AddAuthorizationBuilder();

builder.Services
    .AddFastEndpoints()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddStartupConfiguration();

var app = builder.Build();

app.UseHealthEndpoints();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<ClientEventHub>("/events");
app.UseFastEndpoints();
app.UseCorrelationIdMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

await app.RunAsync();
