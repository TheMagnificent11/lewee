using System.Reflection;
using FreeMediator;
using Lewee.Application;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

namespace Lewee.Application.Tests.Unit;

/// <summary>
/// Test web application factory for testing Lewee.Application behaviors
/// </summary>
public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    public FakeLogger<T> GetLogger<T>() => Services.GetRequiredService<FakeLogCollector>().GetFakeLogger<T>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Add fake logging for testing
            services.AddSingleton<FakeLogCollector>();
            services.AddLogging(builder => builder.AddFakeLogging());

            // Add Application layer dependencies
            var applicationAssembly = typeof(TestCommand).Assembly;
            var domainAssembly = typeof(Lewee.Domain.Entity<>).Assembly;
            
            services.AddApplication(applicationAssembly, domainAssembly);
            services.AddPipelineBehaviors();

            // Add mediator for testing
            services.AddSingleton<IMediator>();
        });

        builder.Configure(app =>
        {
            // Test endpoints
            app.MapPost("/test-command", async (TestCommand command, IMediator mediator, CancellationToken ct) =>
            {
                var result = await mediator.Send(command, ct);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            });

            app.MapPost("/test-domain-exception", async (TestDomainExceptionCommand command, IMediator mediator, CancellationToken ct) =>
            {
                var result = await mediator.Send(command, ct);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            });

            app.MapPost("/test-unhandled-exception", async (TestUnhandledExceptionCommand command, IMediator mediator, CancellationToken ct) =>
            {
                var result = await mediator.Send(command, ct);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            });

            app.MapGet("/test-query", async (IMediator mediator, CancellationToken ct) =>
            {
                var query = new TestQuery(Guid.NewGuid());
                var result = await mediator.Send(query, ct);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            });

            app.MapPost("/test-tenant-command", async (TestTenantCommand command, IMediator mediator, CancellationToken ct) =>
            {
                var result = await mediator.Send(command, ct);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            });

            app.MapPost("/test-server-error", async (TestServerErrorCommand command, IMediator mediator, CancellationToken ct) =>
            {
                var result = await mediator.Send(command, ct);
                return result.IsSuccess ? Results.Ok(result) : Results.StatusCode(500);
            });

            app.MapPost("/test-bad-request", async (TestBadRequestCommand command, IMediator mediator, CancellationToken ct) =>
            {
                var result = await mediator.Send(command, ct);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            });
        });
    }
}

/// <summary>
/// Program class for TestHost
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        // This is required for WebApplicationFactory
    }
}