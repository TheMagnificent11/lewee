using FastEndpoints;
using Lewee.Infrastructure.AspNet.Auth;
using Lewee.Infrastructure.AspNet.Observability;
using Lewee.Infrastructure.AspNet.SignalR;
using Lewee.Infrastructure.Data;
using Lewee.Infrastructure.PostgreSQL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Pizzeria.Common;
using Pizzeria.ServiceDefaults;
using Pizzeria.Store.Application;
using Pizzeria.Store.Data;
using Pizzeria.Store.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var databaseName = ServiceNames.GetPizzaStoreDatabaseName();

builder.Services
    .AddAuthenticatedUserService()
    .AddLeweePostgreSQL<StoreDbContext>(
        builder.Configuration.GetConnectionString(databaseName)!,
        typeof(Pizza).Assembly,
        StoreDbContext.SchemaName)
    .AddLeweeDatabaseServices<StoreDbContext>(typeof(Pizza).Assembly)
    .AddLeweeDatabaseSeeder<StoreDbContext, StoreSeeder>()
    .AddPizzaStoreApplication()
    .AddCorrelationIdServices()
    .AddLeweeSignalR();

// Configure JWT Bearer authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var authority = builder.Configuration["Authentication:Schemes:Bearer:Authority"];
        if (!string.IsNullOrEmpty(authority))
        {
            options.Authority = $"{authority}/realms/pizzeria";
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = $"{authority}/realms/pizzeria",
                ValidateAudience = false,
                ValidateLifetime = true
            };
        }
    });

builder.Services
    .AddAuthorization()
    .AddFastEndpoints()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<ClientEventHub>("/events");
app.UseFastEndpoints();
app.UseCorrelationIdMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

await app.Services.MigrateDatabaseAsync<StoreDbContext>(seedData: true);

await app.RunAsync();
