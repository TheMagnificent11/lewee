using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Lewee.Application;

namespace Lewee.Application.Tests.Unit;

/// <summary>
/// Simple test web application factory for testing Lewee.Application configuration
/// </summary>
public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Add Application layer dependencies
            var applicationAssembly = typeof(TestCommand).Assembly;
            var domainAssembly = typeof(Lewee.Domain.Entity).Assembly;
            
            services.AddApplication(applicationAssembly, domainAssembly);
            services.AddPipelineBehaviors();
        });

        builder.Configure(app =>
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapPost("/test-command", async context =>
                {
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync("OK");
                });
            });
        });
    }
}

/// <summary>
/// Program class for TestHost
/// </summary>
public class Program
{
    // Empty program class for WebApplicationFactory
}