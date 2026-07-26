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

var configuration = builder.AddCSharpApp(ServiceNames.ConfigurationService, "../Pizzeria.Configuration/Pizzeria.Configuration.csproj")
    .WithReference(authServer)
    .WithReference(pizzaStoreDatabase)
    .WaitFor(authServer)
    .WaitFor(pizzaStoreDatabase);

var pizzaStoreApi = builder.AddProject<Projects.Pizzeria_Store_Api>(ServiceNames.PizzaStoreApi)
    .WithReference(pizzaStoreDatabase)
    .WithReference(authServer)
    .WaitForCompletion(configuration)
    .WithHttpHealthCheck("/health");

var pizzaStoreWeb = builder.AddProject<Projects.Pizzeria_Store_Web>(ServiceNames.PizzaStoreWeb)
    .WithReference(pizzaStoreApi)
    .WithReference(authServer)
    .WaitFor(pizzaStoreApi)
    .WithHttpHealthCheck("/health");

if (isTest)
{
    // Environment variables set on the AppHost process (such as IS_INTEGRATION_TEST) are not
    // automatically inherited by project resources launched via DCP, so they must be passed
    // through explicitly for integration tests to enable JSON console logging with scopes.
    pizzaStoreApi.WithEnvironment(Environments.IsIntegrationTestEnvironmentVariableName, "TRUE");
    pizzaStoreWeb.WithEnvironment(Environments.IsIntegrationTestEnvironmentVariableName, "TRUE");
}

var app = builder.Build();

await app.RunAsync();
