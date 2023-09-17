using Lewee.Blazor.Fluxor;
using Lewee.Blazor.Http;
using Lewee.Blazor.Messaging;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Sample.Restaurant.Client;
using Sample.Restaurant.Client.States;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddTransient<CorrelationIdDelegatingHandler>()
    .AddHttpClient<TableClient>(sp => sp.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<CorrelationIdDelegatingHandler>();

builder.Services
    .ConfigureMessageReceiver<MessageToActionMapper>(builder.HostEnvironment.BaseAddress)
    .AddScoped<ITableClient>(provider =>
    {
        return new TableClient(builder.HostEnvironment.BaseAddress, provider.GetService<HttpClient>());
    })
    .ConfigureFluxor()
    .AddMudServices();

await builder.Build().RunAsync();
