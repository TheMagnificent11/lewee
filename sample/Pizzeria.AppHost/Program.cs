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

var pizzeriaDatabase = databaseServer.AddDatabase(ServiceNames.PizzeriaDatabase);

var configuration = builder.AddDotnetProject(ServiceNames.ConfigurationService, "../Pizzeria.Configuration/Pizzeria.Configuration.csproj")
    .WithReference(authServer)
    .WithReference(pizzeriaDatabase)
    .WaitFor(authServer)
    .WaitFor(pizzeriaDatabase);

var authApi = builder.AddDotnetProject(ServiceNames.AuthApi, "../Pizzeria.Auth.Api/Pizzeria.Auth.Api.csproj")
    .WithReference(pizzeriaDatabase)
    .WithReference(authServer)
    .WaitForCompletion(configuration)
    .WithHttpHealthCheck("/health");

var pizzaStoreApi = builder.AddDotnetProject(ServiceNames.PizzaStoreApi, "../Pizzeria.Store.Api/Pizzeria.Store.Api.csproj")
    .WithReference(pizzeriaDatabase)
    .WithReference(authServer)
    .WaitForCompletion(configuration)
    .WithHttpHealthCheck("/health");

var bff = builder.AddDotnetProject(ServiceNames.Bff, "../Pizzeria.Bff/Pizzeria.Bff.csproj")
    .WithReference(authApi)
    .WithReference(pizzaStoreApi)
    .WaitFor(authApi)
    .WaitFor(pizzaStoreApi)
    .WithHttpHealthCheck("/health");

builder.AddDotnetProject(ServiceNames.PizzaStoreWeb, "../Pizzeria.Ordering.Web/Pizzeria.Ordering.Web.csproj")
    .WithReference(bff)
    .WithReference(authServer)
    .WaitFor(bff)
    .WithHttpHealthCheck("/health");

var app = builder.Build();

await app.RunAsync();
