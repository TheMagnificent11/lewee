using Lewee.Blazor;
using Microsoft.AspNetCore.Authentication.Cookies;
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

// Add authentication services using Aspire's Keycloak integration
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "OpenIdConnect";
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddKeycloakOpenIdConnect(
    ServiceNames.AuthServer,
    realm: Pizzeria.Common.Environments.Auth.RealmName,
    options =>
    {
        options.ClientId = Pizzeria.Common.Environments.Auth.Clients.StoreWeb;
        options.ResponseType = "code"; // Use authorization code flow
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.RequireHttpsMetadata = false; // For development/testing only
        options.UsePkce = true; // Enable PKCE for public clients

        // Map claims
        options.TokenValidationParameters.NameClaimType = "preferred_username";
        options.TokenValidationParameters.RoleClaimType = "roles";

        // Add scopes
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// Add Razor components
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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>();

await app.RunAsync();
