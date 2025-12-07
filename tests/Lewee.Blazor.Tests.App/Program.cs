using Aspire.Hosting.Azure;
using Lewee.Blazor.Tests.Contracts;

var builder = DistributedApplication.CreateBuilder(args);

var signalR = builder
    .AddAzureSignalR(ServiceNames.SignalR, AzureSignalRServiceMode.Serverless)
    .RunAsEmulator();

builder.AddProject<Projects.Lewee_Blazor_Tests_Api>(ServiceNames.WebApi)
    .WithReference(signalR)
    .WaitFor(signalR);

await builder.Build().RunAsync();
