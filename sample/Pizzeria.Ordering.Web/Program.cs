using Correlate.Http;
using Lewee.Common;
using Lewee.Infrastructure.Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Pizzeria.Common;
using Pizzeria.Ordering.StateManagement;
using Pizzeria.Ordering.StateManagement.Orders;
using Pizzeria.Ordering.Web;
using Pizzeria.Ordering.Web.Infrastructure;
using Refit;
using CommonEnvironments = Pizzeria.Common.Environments;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var keycloakServiceUrl = Environment.GetEnvironmentVariable(
    $"services__{ServiceNames.AuthServer}__https__0")
    ?? throw new InvalidOperationException("The Aspire gateway did not provide the Keycloak service URL.");
var bffBaseAddress = new Uri(
    new Uri(builder.HostEnvironment.BaseAddress),
    $"_api/{ServiceNames.Bff}/");

builder.Services
    .Configure<CorrelateClientOptions>(options => options.RequestHeader = RequestHeaders.CorrelationId);

builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.Authority =
        $"{keycloakServiceUrl.TrimEnd('/')}/realms/{CommonEnvironments.Auth.RealmName}";
    options.ProviderOptions.ClientId = CommonEnvironments.Auth.Clients.StoreWeb;
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.DefaultScopes.Add("profile");
    options.ProviderOptions.DefaultScopes.Add("email");
});

builder.Services
    .AddCascadingAuthenticationState()
    .AddScoped<BrowserAuthenticatedUserService>()
    .AddScoped<IAuthenticatedUserService>(
        services => services.GetRequiredService<BrowserAuthenticatedUserService>())
    .AddScoped<ClientLifetimeCancellationTokenProvider>()
    .AddScoped<IOrderReconciliationCancellationTokenProvider>(
        services => services.GetRequiredService<ClientLifetimeCancellationTokenProvider>())
    .AddTransient<CorrelatingHttpMessageHandler>()
    .AddScoped(services =>
        new AuthorizationMessageHandler(
                services.GetRequiredService<IAccessTokenProvider>(),
                services.GetRequiredService<NavigationManager>())
            .ConfigureHandler([bffBaseAddress.AbsoluteUri]))
    .AddRefitClient<IBffApiClient>()
    .ConfigureHttpClient(client => client.BaseAddress = bffBaseAddress)
    .AddHttpMessageHandler<CorrelatingHttpMessageHandler>()
    .AddHttpMessageHandler(services => services.GetRequiredService<AuthorizationMessageHandler>());

builder.Services
    .AddAuthorizationCore()
    .AddStoreState(builder.HostEnvironment.IsDevelopment())
    .AddSseMessageReceiver<MessageToActionMapper>(
        client => client.BaseAddress = bffBaseAddress,
        client => client
            .AddHttpMessageHandler<CorrelatingHttpMessageHandler>()
            .AddHttpMessageHandler(services => services.GetRequiredService<AuthorizationMessageHandler>()))
    .AddMudServices();

await builder.Build().RunAsync();
