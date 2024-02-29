using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Sample.Pizzeria.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

//builder.Services.AddRefitClient<IOrdersApi>()
//    .ConfigureHttpClient(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
//    .AddHttpMessageHandler<AuthorizationMessageHandler>();
builder.Services
    .AddSingleton<IOrdersApi, OrdersHttpClient>()
    .AddSingleton<AuthorizationMessageHandler>()
    .AddHttpClient<IOrdersApi, OrdersHttpClient>(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<AuthorizationMessageHandler>();

await builder.Build().RunAsync();
