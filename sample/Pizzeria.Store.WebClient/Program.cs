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

builder.Services
    .ConfigureLeweeBlazor<MessageToActionMapper>(
        builder.HostEnvironment.BaseAddress,
        builder.HostEnvironment.IsDevelopment())
    .AddScoped<IPizzeriaApiClient>(provider =>
    {
        return new PizzeriaApiClient(builder.HostEnvironment.BaseAddress, provider.GetService<HttpClient>());
    })
    .AddHttpClient<PizzeriaApiClient>(sp => sp.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .ConfigureCorrelationIdDelegation();

builder.Services.AddMudServices();

await builder.Build().RunAsync();
