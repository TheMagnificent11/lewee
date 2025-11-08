using FastEndpoints;
using Lewee.Infrastructure.AspNet.Auth;
using Lewee.Infrastructure.AspNet.Observability;
using Lewee.Infrastructure.AspNet.SignalR;
using Lewee.Infrastructure.Data;
using Lewee.Infrastructure.PostgreSQL;
using Pizzeria.Common;
using Pizzeria.ServiceDefaults;
using Pizzeria.Store.Api.Startup;
using Pizzeria.Store.Application;
using Pizzeria.Store.Data;
using Pizzeria.Store.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var databaseName = ServiceNames.GetPizzaStoreDatabaseName();

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

builder.Services
    .AddAuthentication()
    .AddKeycloakJwtBearer(
        serviceName: ServiceNames.AuthServer,
        realm: Pizzeria.Common.Environments.Auth.RealmName,
        options =>
        {
            var isDevOrTest = builder.Environment.IsDevelopment() || Pizzeria.Common.Environments.IsIntegrationTesting;

            // TODO: Fix audience mapping and enable audience validation in production. See issue #1234
            options.TokenValidationParameters.ValidateAudience = !isDevOrTest;
            options.TokenValidationParameters.ValidateIssuer = true;
            options.TokenValidationParameters.ValidateLifetime = true;
            options.TokenValidationParameters.ValidateIssuerSigningKey = true;

            // For development and integration testing - disable HTTPS metadata validation
            // In production, use explicit Authority configuration instead
            if (isDevOrTest)
            {
                options.RequireHttpsMetadata = false;
            }
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
