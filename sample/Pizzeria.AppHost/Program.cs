using Pizzeria.Common;

var builder = DistributedApplication.CreateBuilder(args);

var databaseServer = Environments.IsIntegrationTest
    ? builder.AddPostgres(ServiceNames.DatabaseServer)
    : builder.AddPostgres(ServiceNames.DatabaseServer)
        .WithLifetime(ContainerLifetime.Persistent)
        .WithDataVolume(isReadOnly: false)
        .WithPgWeb();

var pizzaStoreDatabaseName = ServiceNames.GetPizzaStoreDatabaseName();
var pizzaStoreDatabase = databaseServer.AddDatabase(pizzaStoreDatabaseName);

builder.AddProject<Projects.Pizzeria_Store_Api>(ServiceNames.PizzaStoreApi)
    .WithReference(pizzaStoreDatabase)
    .WaitFor(pizzaStoreDatabase);

var app = builder.Build();

await app.RunAsync();
