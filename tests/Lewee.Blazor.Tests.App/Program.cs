using Aspire.Hosting.Azure;
using Lewee.Blazor.Tests.Contracts;

var builder = DistributedApplication.CreateBuilder(args);

var signalR = builder
    .AddAzureSignalR(ServiceNames.SignalR, AzureSignalRServiceMode.Serverless)
    .RunAsEmulator();

builder.AddProject<Projects.Lewee_Blazor_Tests_Api>(ServiceNames.WebApi)
    .WithReference(signalR)
    .WaitFor(signalR);

// Blazor Server web app (uses local SignalR, no Azure dependency)
builder.AddProject<Projects.Lewee_Blazor_Tests_Web>(ServiceNames.BlazorServerWeb);

await builder.Build().RunAsync();
