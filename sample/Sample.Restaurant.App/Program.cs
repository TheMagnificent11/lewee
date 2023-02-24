using Fluxor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Sample.Restaurant.App;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

/*
 * TODO: https://github.com/nblumhardt/serilog-sinks-browserhttp
var appSettings = builder.Configuration.GetSettings<ApplicationSettings>(nameof(ApplicationSettings));
var seqSettings = builder.Configuration.GetSettings<SeqSettings>(nameof(SeqSettings));

builder.Host.ConfigureLogging(appSettings, seqSettings);
*/

var apiUrl = builder.Configuration.GetConnectionString("Api");
if (string.IsNullOrWhiteSpace(apiUrl))
{
    throw new ApplicationException("Could not find API URL");
}

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(apiUrl)
    });

builder.Services.AddFluxor(options =>
{
    options.ScanAssemblies(typeof(Program).Assembly);

#if DEBUG
    options.UseReduxDevTools();
#endif
});

builder.Services.Scan(scan => scan
    .FromAssemblyOf<ITableClient>()
    .AddClasses().AsImplementedInterfaces()
    .WithScopedLifetime());

await builder.Build().RunAsync();
