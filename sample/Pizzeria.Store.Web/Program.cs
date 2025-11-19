using Lewee.Blazor;
using MudBlazor.Services;
using Pizzeria.Common;
using Pizzeria.ServiceDefaults;
using Pizzeria.Store.Web;
using Pizzeria.Store.Web.Services;
using Pizzeria.Store.Web.States;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults
builder.AddServiceDefaults();

// Add services to the container
builder.Services.AddRazorComponents();

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
app.UseAntiforgery();

app.MapRazorComponents<App>();

await app.RunAsync();
