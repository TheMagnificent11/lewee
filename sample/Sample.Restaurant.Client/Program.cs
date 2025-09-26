using Lewee.Blazor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Sample.Restaurant.Client;
using Sample.Restaurant.Client.States;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .ConfigureLeweeBlazor<MessageToActionMapper>(
        builder.HostEnvironment.BaseAddress,
        builder.HostEnvironment.IsDevelopment())
    .AddScoped<ITableClient>(provider =>
    {
        return new TableClient(builder.HostEnvironment.BaseAddress, provider.GetService<HttpClient>());
    })
    .AddHttpClient<TableClient>(sp => sp.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .ConfigureCorrelationIdDelegation();

builder.Services.AddMudServices();

await builder.Build().RunAsync();
