using Lewee.Blazor.Messaging;
using Lewee.Infrastructure.AspNet.Observability;
using Lewee.Infrastructure.AspNet.SignalR;
using Lewee.Infrastructure.Auth;
using Lewee.Infrastructure.Data;
using Lewee.Infrastructure.PostgreSQL;
using MudBlazor.Services;
using Pizzeria.Common;
using Pizzeria.ServiceDefaults;
using Pizzeria.Store.Application;
using Pizzeria.Store.Contracts;
using Pizzeria.Store.Data;
using Pizzeria.Store.Domain;
using Pizzeria.Store.StateManagement;
using Pizzeria.Store.Web;
using Pizzeria.Store.Web.Infrastructure;

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
    .AddAuth()
    .AddLeweeSignalR()
    .AddDatabaseHealthCheck();

// Client services
builder.Services
    .AddStoreState(builder.Environment.IsDevelopment())
    .AddSignalRMessageReceiver<MessageToActionMapper>()
    .AddMudServices()
    .AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

app.UseHealthEndpoints();

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

app.MapLeweeSignalRHub();

app.MapStaticAssets();

app
    .MapSignOut()
    .MapRazorComponents<App>()
    .AddAdditionalAssemblies(typeof(Pizzeria.Store.Components._Imports).Assembly)
    .AddInteractiveServerRenderMode();

await app.RunAsync();
