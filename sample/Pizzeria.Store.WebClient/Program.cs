using Lewee.Blazor;
using MudBlazor.Services;
using Pizzeria.Common;
using Pizzeria.ServiceDefaults;
using Pizzeria.Store.WebClient.Services;
using Pizzeria.Store.WebClient.States;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults (now works with server-side Blazor!)
builder.AddServiceDefaults();

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Configure Refit HTTP client for API using Aspire service discovery
const string ApiClientName = "PizzeriaApi";
builder.Services
    .AddRefitClient<IPizzeriaApiClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri($"https://{ServiceNames.PizzaStoreApi}"))
    .AddCorrelationIdDelegationHandler();

// Register the same HttpClient configuration for SignalR service discovery
builder.Services
    .AddHttpClient(ApiClientName, c => c.BaseAddress = new Uri($"https://{ServiceNames.PizzaStoreApi}"));

// Configure Lewee.Blazor to use service discovery for SignalR connections
builder.Services.AddLeweeBlazor<MessageToActionMapper>(
    ApiClientName,
    builder.Environment.IsDevelopment());

builder.Services.AddMudServices();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.MapDefaultEndpoints();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await app.RunAsync();
