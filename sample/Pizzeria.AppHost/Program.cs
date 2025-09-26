using Pizzeria.Common;

var builder = DistributedApplication.CreateBuilder(args);

var databaseServer = builder.AddPostgres(ServiceNames.DatabaseServer)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume(isReadOnly: false)
    .WithPgWeb();

var pizzaStoreDatabase = databaseServer.AddDatabase(ServiceNames.PizzaStoreDatabase);

builder.AddProject<Projects.Pizzeria_Store_Api>(ServiceNames.PizzaStoreApi)
    .WithReference(pizzaStoreDatabase)
    .WaitFor(pizzaStoreDatabase);

var app = builder.Build();

await app.RunAsync();
