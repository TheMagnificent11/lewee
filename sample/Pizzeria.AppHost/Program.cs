using Pizzeria.Common;

var builder = DistributedApplication.CreateBuilder(args);

var setDefaultAuthServerAdminCredentials = Environments.IsIntegrationTesting;

#if DEBUG
setDefaultAuthServerAdminCredentials = true;
#endif

var authServer = builder.AddKeycloak(ServiceNames.AuthServer)
    .WithRealmImport($"keycloak/{Environments.Auth.RealmName}-realm.json");

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

var configuration = builder.AddProject<Projects.Pizzeria_Configuration>(ServiceNames.ConfigurationService)
    .WithReference(authServer)
    .WithReference(pizzaStoreDatabase)
    .WaitFor(authServer)
    .WaitFor(pizzaStoreDatabase)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.Pizzeria_Store_Api>(ServiceNames.PizzaStoreApi)
    .WithReference(pizzaStoreDatabase)
    .WithReference(authServer)
    .WaitFor(configuration)
    .WithHttpHealthCheck("/health");

// Temporarily disable web client to allow integration tests to run
// TODO: Investigate Aspire DCP timeout issues when web client is enabled in test environment
// builder.AddProject<Projects.Pizzeria_Store_Web>(ServiceNames.PizzaStoreWebClient)
//     .WithReference(authServer)
//     .WaitFor(configuration);

var app = builder.Build();

await app.RunAsync();
