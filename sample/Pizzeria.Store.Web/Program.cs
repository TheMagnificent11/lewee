using Lewee.Infrastructure.Fluxor;
using Lewee.Infrastructure.Keycloak;
using Lewee.Infrastructure.Refit;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using MudBlazor.Services;
using Pizzeria.Common;
using Pizzeria.ServiceDefaults;
using Pizzeria.Store.Contracts;
using Pizzeria.Store.StateManagement;
using Pizzeria.Store.Web;
using Pizzeria.Store.Web.Infrastructure;
using CommonEnvironments = Pizzeria.Common.Environments;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services
    .AddWebApiHttpClient<IStoreApiClient>(ServiceNames.PizzaStoreApi)
    .AddKeycloakAuthenticationForWebApp(
        keycloakServiceName: ServiceNames.AuthServer,
        keycloakRealmName: CommonEnvironments.Auth.RealmName,
        keycloakClientId: CommonEnvironments.Auth.Clients.StoreWeb,
        events: new OpenIdConnectEvents
        {
            OnTokenValidated = async context => await context.CreateCustomerOnFirstLoginAsync(),
        });

builder.Services
    .AddStoreState(builder.Environment.IsDevelopment())
    .AddSseMessageReceiver<MessageToActionMapper>(client =>
    {
        client.BaseAddress = new Uri($"https://{ServiceNames.PizzaStoreApi}");
    })
    .AddMudServices()
    .AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(PageRoutes.Error, createScopeForErrors: true);
    app.UseHsts();
}

app
    .UseAntiforgery()
    .UseHttpsRedirection()
    .UseAuthentication()
    .UseAuthorization();

app.MapStaticAssets();

app.MapKeycloakSignOut(PageRoutes.SignOut);

app
    .MapRazorComponents<App>()
    .AddAdditionalAssemblies(typeof(Pizzeria.Store.Components._Imports).Assembly)
    .AddInteractiveServerRenderMode();

await app.RunAsync();
