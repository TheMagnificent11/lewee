using Lewee.Infrastructure.Data.Tests.App;

var builder = DistributedApplication.CreateBuilder(args);

builder
    .AddPostgres(ServiceNames.DatabaseServer)
    .AddDatabase(ServiceNames.Database);

await builder.Build().RunAsync();
