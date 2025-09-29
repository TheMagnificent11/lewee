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

// Get API base URL from configuration (injected by Aspire AppHost)
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7062";

Console.WriteLine($"Using API Base URL: {apiBaseUrl}");

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
