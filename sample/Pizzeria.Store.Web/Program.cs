using MudBlazor.Services;
using Pizzeria.ServiceDefaults;
using Pizzeria.Store.Components;
using Pizzeria.Store.Web;
using Pizzeria.Store.Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services
    .AddAuth()
    .AddCascadingAuthenticationState()
    .AddMudServices()
    .AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(PageRoutes.Error, createScopeForErrors: true);

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app
    .UseHttpsRedirection()
    .UseAntiforgery()
    .UseAuthentication()
    .UseAuthorization();

app.MapStaticAssets();

app
    .MapSignOut()
    .MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
