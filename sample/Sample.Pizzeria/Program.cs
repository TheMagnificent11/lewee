using FastEndpoints;
using FastEndpoints.Swagger;
using Lewee.Infrastructure.AspNet.Auth;
using Lewee.Infrastructure.AspNet.Observability;
using Lewee.Infrastructure.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sample.Identity.Domain;
using Sample.Identity.Infrastructure;
using Sample.Pizzeria.Application;
using Sample.Pizzeria.Components;
using Sample.Pizzeria.Components.Account;
using Sample.Pizzeria.Infrastructure.Data;
using Serilog;
using DomainOrder = Sample.Pizzeria.Domain.Order;

var builder = WebApplication.CreateBuilder(args);

var identityConnectionString = builder.Configuration.GetConnectionString("Identity")
    ?? throw new InvalidOperationException("Connection string 'Identity' not found.");
var pizzeriaConnectionString = builder.Configuration.GetConnectionString("Pizzeria")
    ?? throw new InvalidOperationException("Connection string 'Pizzeria' not found.");

var logger = builder.Environment!.ConfigureLogging(builder.Configuration);
builder.Host.UseSerilog(logger, dispose: true);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddDbContext<ApplicationIdentityDbContext>(options => options.UseSqlServer(identityConnectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
    .AddApiEndpoints()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.ConfigureDatabase<PizzeriaDbContext>(pizzeriaConnectionString, typeof(DomainOrder).Assembly);

builder.Services.AddPizzeriaApplication();

builder.Services
    .ConfigureAuthenticatedUserService()
    .AddCorrelationIdServices()
    .AddHealthChecks()
    .AddDbContextCheck<ApplicationIdentityDbContext>()
    .AddDbContextCheck<PizzeriaDbContext>();

builder.Services
    .AddFastEndpoints()
    .SwaggerDocument(x =>
    {
        x.DocumentSettings = y =>
        {
            y.Title = "Pizzeria API";
            y.Version = "v1";
        };
    });

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCorrelationIdMiddleware();
app.UseHealthChecks("/health");

app.UseStaticFiles();
app.UseAntiforgery();

app.UseFastEndpoints();
app.UseSwaggerGen();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Sample.Pizzeria.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

await app.Services.MigrateDatabase<ApplicationIdentityDbContext>();
await app.Services.MigrateDatabase<PizzeriaDbContext>();

app.Run();
