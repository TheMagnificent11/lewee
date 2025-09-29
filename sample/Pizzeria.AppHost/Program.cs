using Pizzeria.Common;

var builder = DistributedApplication.CreateBuilder(args);

var databaseServer = Environments.IsIntegrationTesting
    ? builder.AddPostgres(ServiceNames.DatabaseServer)
    : builder.AddPostgres(ServiceNames.DatabaseServer)
        .WithLifetime(ContainerLifetime.Persistent)
        .WithDataVolume(isReadOnly: false)
        .WithPgWeb();

var pizzaStoreDatabaseName = ServiceNames.GetPizzaStoreDatabaseName();
var pizzaStoreDatabase = databaseServer.AddDatabase(pizzaStoreDatabaseName);

var pizzaStoreApi = builder.AddProject<Projects.Pizzeria_Store_Api>(ServiceNames.PizzaStoreApi)
    .WithReference(pizzaStoreDatabase)
    .WaitFor(pizzaStoreDatabase);

builder.AddProject<Projects.Pizzeria_Store_WebClient>(ServiceNames.PizzaStoreWebClient)
    .WithReference(pizzaStoreApi)
    .WithEnvironment("ApiBaseUrl", pizzaStoreApi.GetEndpoint("https"))
    .WaitFor(pizzaStoreApi);

var app = builder.Build();

await app.RunAsync();
