using Lewee.Blazor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Pizzeria.Store.WebClient;
using Pizzeria.Store.WebClient.Services;
using Pizzeria.Store.WebClient.States;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Use the API server URL for SignalR and HTTP client
var apiBaseUrl = builder.Configuration["services:pizza-store-api:https:0"] 
    ?? builder.Configuration["services:pizza-store-api:http:0"] 
    ?? "https://localhost:7062"; // fallback for local development

builder.Services
    .ConfigureLeweeBlazor<MessageToActionMapper>(
        apiBaseUrl,
        builder.HostEnvironment.IsDevelopment())
    .AddScoped<IPizzeriaApiClient>(provider =>
    {
        return new PizzeriaApiClient(apiBaseUrl, provider.GetService<HttpClient>());
    })
    .AddHttpClient<PizzeriaApiClient>(sp => sp.BaseAddress = new Uri(apiBaseUrl))
    .ConfigureCorrelationIdDelegation();

builder.Services.AddMudServices();

await builder.Build().RunAsync();
