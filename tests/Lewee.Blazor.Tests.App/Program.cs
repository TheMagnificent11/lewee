using Aspire.Hosting.Azure;
using Lewee.Blazor.Tests.App;

var builder = DistributedApplication.CreateBuilder(args);

builder
    .AddAzureSignalR(ServiceNames.SignalR, AzureSignalRServiceMode.Serverless)
    .RunAsEmulator();

await builder.Build().RunAsync();
