using Lewee.Infrastructure.AspNet.Auth;
using Lewee.Infrastructure.AspNet.Observability;
using Lewee.Infrastructure.Data;
using Lewee.Infrastructure.PostgreSQL;
using MudBlazor.Services;
using Pizzeria.Common;
using Pizzeria.ServiceDefaults;
using Pizzeria.Store.Application;
using Pizzeria.Store.Components;
using Pizzeria.Store.Data;
using Pizzeria.Store.Domain;
using Pizzeria.Store.Web;
using Pizzeria.Store.Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var databaseName = ServiceNames.PizzaStoreDatabaseName;

// Server services
builder.Services
    .AddAuthenticatedUserService()
    .AddLeweePostgreSQL<StoreDbContext>(
        builder.Configuration.GetConnectionString(databaseName)!,
        typeof(Pizza).Assembly,
        StoreDbContext.SchemaName)
    .AddLeweeDatabaseServices<StoreDbContext>(typeof(Pizza).Assembly)
    .AddPizzaStoreApplication()
    .AddCorrelationIdServices()
    .AddAuth()
    .AddDatabaseHealthCheck();

// Client services
builder.Services
    .AddMudServices()
    .AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(PageRoutes.Error, createScopeForErrors: true);

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app
    .UseAntiforgery()
    .UseHttpsRedirection()
    .UseCorrelationIdMiddleware()
    .UseAuthentication()
    .UseAuthorization();

app.MapStaticAssets();

app
    .MapSignOut()
    .MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
