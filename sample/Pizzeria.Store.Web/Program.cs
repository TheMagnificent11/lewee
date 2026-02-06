using Lewee.Infrastructure.Fluxor;
using Lewee.Infrastructure.Keycloak;
using Lewee.Infrastructure.Refit;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Web;
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
    .AddHttpContextAccessor()
    .AddWebApiHttpClient<IStoreApiClient>(ServiceNames.PizzaStoreApi)
    .AddKeycloakAuthenticationForWebApp(
        keycloakServiceName: ServiceNames.AuthServer,
        keycloakRealmName: CommonEnvironments.Auth.RealmName,
        keycloakClientId: CommonEnvironments.Auth.Clients.StoreWeb,
        events: new OpenIdConnectEvents
        {
            OnTokenValidated = async context => await context.CreateCustomerOnFirstLoginAsync(),
        });

builder.Services.AddScoped<AccessTokenService, ServerAccessTokenService>();

builder.Services
    .AddStoreState(builder.Environment.IsDevelopment())
    .AddMudServices()
    .AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .RegisterPersistentService<AccessTokenService>(RenderMode.InteractiveWebAssembly);

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler(PageRoutes.Error, createScopeForErrors: true);
    app.UseHsts();
}

app
    .UseHttpsRedirection()
    .UseAntiforgery()
    .UseAuthentication()
    .UseAuthorization();

app.MapStaticAssets();

app.MapKeycloakSignOut(PageRoutes.SignOut);

app
    .MapRazorComponents<App>()
    .AddAdditionalAssemblies(typeof(Pizzeria.Store.Components._Imports).Assembly)
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();

await app.RunAsync();
