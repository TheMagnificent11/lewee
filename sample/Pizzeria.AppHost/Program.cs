using Pizzeria.Common;

var builder = DistributedApplication.CreateBuilder(args);

var setDefaultAuthServerAdminCredentials = Environments.IsIntegrationTesting;

#if DEBUG
setDefaultAuthServerAdminCredentials = true;
#endif

var authServer = builder.AddKeycloak(ServiceNames.AuthServer);

if (setDefaultAuthServerAdminCredentials)
{
    authServer = authServer
        .WithEnvironment("KC_BOOTSTRAP_ADMIN_USERNAME", Environments.Auth.DefaultAdminCredentialsForTesting.Username)
        .WithEnvironment("KC_BOOTSTRAP_ADMIN_PASSWORD", Environments.Auth.DefaultAdminCredentialsForTesting.Password);
}

if (!Environments.IsIntegrationTesting)
{
    authServer = authServer
        .WithLifetime(ContainerLifetime.Persistent)
        .WithDataVolume();
}

var databaseServer = Environments.IsIntegrationTesting
    ? builder.AddPostgres(ServiceNames.DatabaseServer)
    : builder.AddPostgres(ServiceNames.DatabaseServer)
        .WithLifetime(ContainerLifetime.Persistent)
        .WithDataVolume(isReadOnly: false)
        .WithPgWeb();

var pizzaStoreDatabaseName = ServiceNames.GetPizzaStoreDatabaseName();
var pizzaStoreDatabase = databaseServer.AddDatabase(pizzaStoreDatabaseName);

var configuration = builder.AddProject<Projects.Pizzeria_Configuration>("pizzeria-configuration")
    .WithReference(authServer)
    .WithReference(pizzaStoreDatabase)
    .WaitFor(authServer)
    .WaitFor(pizzaStoreDatabase);

builder.AddProject<Projects.Pizzeria_Store_Api>(ServiceNames.PizzaStoreApi)
    .WithReference(pizzaStoreDatabase)
    .WithReference(authServer)
    .WithEnvironment(Environments.AuthenticationSchemesBearerValidAudience, ServiceNames.PizzaStoreApi)
    .WaitFor(configuration)
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
