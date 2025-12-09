using FastEndpoints;
using Lewee.Infrastructure.AspNet.Auth;
using Lewee.Infrastructure.AspNet.Observability;
using Lewee.Infrastructure.AspNet.SignalR;
using Lewee.Infrastructure.Data;
using Lewee.Infrastructure.PostgreSQL;
using Pizzeria.Common;
using Pizzeria.ServiceDefaults;
using Pizzeria.Store.Api.Infrastructure;
using Pizzeria.Store.Application;
using Pizzeria.Store.Data;
using Pizzeria.Store.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var databaseName = ServiceNames.PizzaStoreDatabaseName;
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
    .AddAuth(isDevOrTest)
    .AddLeweeSignalR(builder.Configuration.GetConnectionString(ServiceNames.SignalR)!)
    .AddFastEndpoints()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddDatabaseHealthCheck();

var app = builder.Build();

app.UseHealthEndpoints();
app.UseAuthentication();
app.UseAuthorization();
app.MapLeweeSignalRNegotiateEndpoint();
app.UseFastEndpoints();
app.UseCorrelationIdMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

await app.RunAsync();
