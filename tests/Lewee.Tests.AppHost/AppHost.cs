var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Lewee_Tests_Api>("lewee-tests-api");

builder.AddProject<Projects.Lewee_Tests_Web>("lewee-tests-web");

builder.Build().Run();
