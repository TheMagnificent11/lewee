var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Lewee_Tests_Api>("lewee-tests-api");

builder.Build().Run();
