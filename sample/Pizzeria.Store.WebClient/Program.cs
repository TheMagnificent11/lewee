using Lewee.Blazor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Pizzeria.Store.WebClient;
using Pizzeria.Store.WebClient.Services;
using Pizzeria.Store.WebClient.States;
using Refit;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Use Aspire service discovery for API base URL
var apiBaseUrl = builder.Configuration["services:pizza-store-api:https:0"] 
    ?? builder.Configuration["services:pizza-store-api:http:0"] 
    ?? "https://localhost:7062"; // fallback for local development

// Configure Lewee.Blazor with proper SignalR URL
builder.Services
    .ConfigureLeweeBlazor<MessageToActionMapper>(
        apiBaseUrl,
        builder.HostEnvironment.IsDevelopment());

// Configure Refit HTTP client for API
builder.Services
    .AddRefitClient<IPizzeriaApiClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl))
    .ConfigureCorrelationIdDelegation();

builder.Services.AddMudServices();

await builder.Build().RunAsync();
