using Lewee.Blazor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;
using Pizzeria.Common;
using Pizzeria.Store.WebClient;
using Pizzeria.Store.WebClient.Services;
using Pizzeria.Store.WebClient.States;
using Refit;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.AddServiceDefaults();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBasAddress = new Uri($"https+http://{ServiceNames.PizzaStoreApi}");

builder.Services.ConfigureLeweeBlazor<MessageToActionMapper>(
    apiBasAddress,
    builder.HostEnvironment.IsDevelopment());

builder.Services
    .AddRefitClient<IPizzeriaApiClient>()
    .ConfigureHttpClient(c => c.BaseAddress = apiBasAddress)
    .ConfigureCorrelationIdDelegation();

builder.Services.AddMudServices();

await builder.Build().RunAsync();
