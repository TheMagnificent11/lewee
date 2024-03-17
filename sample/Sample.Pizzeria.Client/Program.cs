using Lewee.Blazor;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Refit;
using Sample.Pizzeria.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

builder.Services
    .ConfigureLeweeBlazor<MessageToActionMapper>(
        builder.HostEnvironment.BaseAddress,
        builder.HostEnvironment.IsDevelopment());

builder.Services
    .AddRefitClient<IOrdersApi>()
    .ConfigureHttpClient(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .ConfigureCorrelationIdDelegation();

await builder.Build().RunAsync();
