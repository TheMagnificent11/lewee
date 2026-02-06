using Lewee.Infrastructure.Fluxor;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Pizzeria.Common;
using Pizzeria.Store.StateManagement;
using Pizzeria.Store.Web.Infrastructure;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services
    .AddPersistentStateAccessToken()
    .AddWebApiHttpClientForWasm<IStoreApiClient>(ServiceNames.PizzaStoreApi)
    .AddStoreState(builder.HostEnvironment.IsDevelopment())
    .AddSseMessageReceiver<MessageToActionMapper>(client =>
    {
        client.BaseAddress = new Uri($"https://{ServiceNames.PizzaStoreApi}");
    })
    .AddMudServices();

await builder.Build().RunAsync();
