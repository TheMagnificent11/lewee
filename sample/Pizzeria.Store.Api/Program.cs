using FastEndpoints;
using Lewee.Infrastructure.AspNet.Auth;
using Lewee.Infrastructure.AspNet.Observability;
using Lewee.Infrastructure.AspNet.SignalR;
using Lewee.Infrastructure.Data;
using Lewee.Infrastructure.PostgreSQL;
using Pizzeria.Common;
using Pizzeria.ServiceDefaults;
using Pizzeria.Store.Application;
using Pizzeria.Store.Data;
using Pizzeria.Store.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services
    .AddAuthenticatedUserService()
    .AddLeweePostgreSQL<StoreDbContext>(
        builder.Configuration.GetConnectionString(ServiceNames.PizzaStoreDatabase)!,
        typeof(Pizza).Assembly,
        StoreDbContext.SchemaName)
    .AddLeweeDatabaseServices<StoreDbContext>(typeof(Pizza).Assembly)
    .AddLeweeDatabaseSeeder<StoreDbContext, StoreSeeder>()
    .AddPizzaStoreApplication()
    .AddCorrelationIdServices()
    .ConfigureSignalR()
    .AddFastEndpoints()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseFastEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

await app.Services.MigrateDatabaseAsync<StoreDbContext>(seedData: true);

await app.RunAsync();
