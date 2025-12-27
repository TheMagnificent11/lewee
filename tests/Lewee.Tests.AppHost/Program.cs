using Lewee.Tests.Contracts;

var builder = DistributedApplication.CreateBuilder(args);

builder
    .AddPostgres(ServiceNames.DatabaseServer)
    .AddDatabase(ServiceNames.Database);

builder.AddProject<Projects.Lewee_Tests_Api>(ServiceNames.WebApi);

await builder.Build().RunAsync();
