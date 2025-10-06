using Pizzeria.Common;

var builder = DistributedApplication.CreateBuilder(args);

// Add Keycloak container manually for better control
var keycloak = builder.AddContainer(ServiceNames.Keycloak, "quay.io/keycloak/keycloak", "26.1.0")
    .WithEndpoint(port: 8080, targetPort: 8080, name: "http")
    .WithEnvironment("KEYCLOAK_ADMIN", "admin")
    .WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", "admin")
    .WithArgs("start-dev");

var databaseServer = Environments.IsIntegrationTesting
    ? builder.AddPostgres(ServiceNames.DatabaseServer)
    : builder.AddPostgres(ServiceNames.DatabaseServer)
        .WithLifetime(ContainerLifetime.Persistent)
        .WithDataVolume(isReadOnly: false)
        .WithPgWeb();

var pizzaStoreDatabaseName = ServiceNames.GetPizzaStoreDatabaseName();
var pizzaStoreDatabase = databaseServer.AddDatabase(pizzaStoreDatabaseName);

builder.AddProject<Projects.Pizzeria_Store_Api>(ServiceNames.PizzaStoreApi)
    .WithReference(pizzaStoreDatabase)
    .WithEnvironment("Authentication__Schemes__Bearer__Authority", keycloak.GetEndpoint("http"))
    .WithEnvironment("Authentication__Schemes__Bearer__ValidAudience", "pizzeria-store-api")
    .WithEnvironment("Authentication__Schemes__Bearer__ValidIssuer", keycloak.GetEndpoint("http"))
    .WithEnvironment("Authentication__Schemes__Bearer__RequireHttpsMetadata", "false")
    .WaitFor(pizzaStoreDatabase)
    .WaitFor(keycloak)
    .WithHttpHealthCheck("/health");

/*
 * Saji (202510-07)
 * --------------------------------------------------------------------------------------
 * The server-rendered Blazor app has a bug receiving SignalR messages.
 *
 * The preference is to use Blazor WASM for the web client project, but there is bug with
 * Blazor WASM Aspire service discovery (https://github.com/dotnet/aspire/issues/8486).
 *
 * Re-enable this when both of the above issues are fixed.
 * https://github.com/TheMagnificent11/lewee/issues/372
 *
builder.AddProject<Projects.Pizzeria_Store_WebClient>(ServiceNames.PizzaStoreWebClient)
    .WithReference(pizzaStoreApi)
    .WaitFor(pizzaStoreApi)
    .WithHttpHealthCheck("/health");
*/

var app = builder.Build();

await app.RunAsync();
