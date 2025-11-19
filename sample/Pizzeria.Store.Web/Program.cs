using System.Security.Claims;
using Lewee.Blazor;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MudBlazor.Services;
using Pizzeria.Common;
using Pizzeria.ServiceDefaults;
using Pizzeria.Store.Contracts;
using Pizzeria.Store.Web;
using Pizzeria.Store.Web.Services;
using Pizzeria.Store.Web.States;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults
builder.AddServiceDefaults();

// Add authentication services
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Authority = $"https://{ServiceNames.AuthServer}/realms/{Pizzeria.Common.Environments.Auth.RealmName}";
    options.ClientId = "pizzeria-store-web";
    options.ResponseType = OpenIdConnectResponseType.Code;
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.RequireHttpsMetadata = false; // For development/testing only

    // Map claims
    options.TokenValidationParameters.NameClaimType = "preferred_username";
    options.TokenValidationParameters.RoleClaimType = "roles";

    // Add scopes
    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");

    // Handle user creation on first login
    options.Events = new OpenIdConnectEvents
    {
        OnTokenValidated = async context =>
        {
            var externalUserId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(externalUserId))
            {
                var apiClient = context.HttpContext.RequestServices.GetRequiredService<IPizzeriaApiClient>();
                try
                {
                    // Try to create the user - if they already exist, the API will return an error which we ignore
                    await apiClient.CreateCustomerAsync(new CreateCustomerRequest
                    {
                        ExternalUserId = externalUserId,
                    });
                }
                catch
                {
                    // Ignore errors - user might already exist
                }
            }
        },
    };
});

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
