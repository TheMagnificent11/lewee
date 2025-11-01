using Lewee.Infrastructure.Data.IntegrationAppHost;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder
    .AddPostgres(ServiceNames.DatabaseServer)
    .AddDatabase(ServiceNames.Database);

await builder.Build().RunAsync();
