using Pizzeria.Common;

var builder = DistributedApplication.CreateBuilder(args);

var isTest = Environments.IsIntegrationTesting;
var isDevOrTest = isTest;

#if DEBUG
isDevOrTest = true;
#endif

var authServer = builder.AddKeycloak(ServiceNames.AuthServer)
    .WithRealmImport($"keycloak/{Environments.Auth.RealmName}-realm.json");

if (isDevOrTest)
{
    authServer = authServer
        .WithEnvironment("KC_BOOTSTRAP_ADMIN_USERNAME", Environments.Auth.DefaultAdminCredentialsForTesting.Username)
        .WithEnvironment("KC_BOOTSTRAP_ADMIN_PASSWORD", Environments.Auth.DefaultAdminCredentialsForTesting.Password);
}

if (!isTest)
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

var pizzaStoreDatabaseName = ServiceNames.PizzaStoreDatabaseName;
var pizzaStoreDatabase = databaseServer.AddDatabase(pizzaStoreDatabaseName);

var configuration = builder.AddProject<Projects.Pizzeria_Configuration>(ServiceNames.ConfigurationService)
    .WithReference(authServer)
    .WithReference(pizzaStoreDatabase)
    .WaitFor(authServer)
    .WaitFor(pizzaStoreDatabase)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.Pizzeria_Store_Web>(ServiceNames.PizzaStoreWeb)
    .WithReference(pizzaStoreDatabase)
    .WithReference(authServer)
    .WaitFor(configuration)
    .WithHttpHealthCheck("/health");

var app = builder.Build();

await app.RunAsync();
