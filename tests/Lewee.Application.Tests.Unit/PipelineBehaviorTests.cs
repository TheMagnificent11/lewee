using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FreeMediator;
using Lewee.Application.Mediation.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Xunit;

namespace Lewee.Application.Tests.Unit;

/// <summary>
/// Tests for pipeline behaviors using TestServer
/// </summary>
public class PipelineBehaviorTests
{
    [Fact]
    public async Task ValidationBehavior_WithInvalidCommand_ShouldReturnBadRequestAsync()
    {
        // Arrange
        using var testServer = CreateTestServer(endpoints =>
        {
            endpoints.MapPost("/test-command", async (TestCommand command, IMediator mediator, CancellationToken ct) =>
            {
                var result = await mediator.Send(command, ct);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            });
        });

        using var client = testServer.CreateClient();
        var logCollector = testServer.Services.GetRequiredService<FakeLogCollector>();
        var invalidCommand = new TestCommand(string.Empty, Guid.NewGuid()); // Empty name should fail validation

        // Act
        using var response = await client.PostAsJsonAsync("/test-command", invalidCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Assert logs - ValidationBehavior should not log directly, but failure logging might occur
        var logs = logCollector.GetSnapshot();
        // The behavior validation happens and returns BadRequest without throwing exceptions
        logs.Should().NotBeEmpty(); // Some logs should be present from pipeline execution
    }

    [Fact]
    public async Task ValidationBehavior_WithValidCommand_ShouldReturnOkAsync()
    {
        // Arrange
        using var testServer = CreateTestServer(endpoints =>
        {
            endpoints.MapPost("/test-command", async (TestCommand command, IMediator mediator, CancellationToken ct) =>
            {
                var result = await mediator.Send(command, ct);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            });
        });

        using var client = testServer.CreateClient();
        var logCollector = testServer.Services.GetRequiredService<FakeLogCollector>();
        var validCommand = new TestCommand("Valid Name", Guid.NewGuid());

        // Act
        using var response = await client.PostAsJsonAsync("/test-command", validCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert logs - Should contain performance and correlation logging
        var logs = logCollector.GetSnapshot();
        logs.Should().Contain(log => log.Message.Contains("Beginning operation")); // Performance behavior
        logs.Should().Contain(log => log.Message.Contains("Completed operation")); // Performance behavior
    }

    [Fact]
    public async Task DomainExceptionBehavior_WithDomainException_ShouldReturnBadRequestAsync()
    {
        // Arrange
        using var testServer = CreateTestServer(endpoints =>
        {
            endpoints.MapPost("/test-domain-exception", async (TestDomainExceptionCommand command, IMediator mediator, CancellationToken ct) =>
            {
                var result = await mediator.Send(command, ct);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            });
        });

        using var client = testServer.CreateClient();
        var logCollector = testServer.Services.GetRequiredService<FakeLogCollector>();
        var command = new TestDomainExceptionCommand(Guid.NewGuid());

        // Act
        using var response = await client.PostAsJsonAsync("/test-domain-exception", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Assert logs - DomainExceptionBehavior should log Information level when catching domain exceptions
        var logs = logCollector.GetSnapshot();
        logs.Should().Contain(log => log.Level == LogLevel.Information &&
                                    log.Message.Contains("Domain exception caught"));
    }

    [Fact]
    public async Task PerformanceBehavior_ShouldLogTimingAsync()
    {
        // Arrange
        using var testServer = CreateTestServer(endpoints =>
        {
            endpoints.MapPost("/test-command", async (TestCommand command, IMediator mediator, CancellationToken ct) =>
            {
                var result = await mediator.Send(command, ct);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            });
        });

        using var client = testServer.CreateClient();
        var logCollector = testServer.Services.GetRequiredService<FakeLogCollector>();
        var command = new TestCommand("Valid Name", Guid.NewGuid());

        // Act
        using var response = await client.PostAsJsonAsync("/test-command", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert performance logs
        var logs = logCollector.GetSnapshot();
        logs.Should().Contain(log => log.Message.Contains("Beginning operation Lewee.Application.Tests.Unit.TestCommand Handler"));
        logs.Should().Contain(log => log.Message.Contains("Completed operation Lewee.Application.Tests.Unit.TestCommand Handler") &&
                                    log.Message.Contains("ms"));
    }

    [Fact]
    public async Task FailureLoggingBehavior_WithFailure_ShouldLogFailureAsync()
    {
        // Arrange
        using var testServer = new TestServer(new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddRouting(); // Required for UseRouting()
                services.AddFakeLogging();
                var applicationAssembly = typeof(TestBadRequestCommand).Assembly;
                var domainAssembly = typeof(Lewee.Domain.Entity).Assembly;
                services.AddApplication(applicationAssembly, domainAssembly);
                services.AddPipelineBehaviors(); // This includes FailureLoggingBehavior
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapPost("/test-bad-request", async (TestBadRequestCommand command, IMediator mediator, CancellationToken ct) =>
                    {
                        var result = await mediator.Send(command, ct);
                        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
                    });
                });
            }));

        using var client = testServer.CreateClient();
        var logCollector = testServer.Services.GetRequiredService<FakeLogCollector>();
        var command = new TestBadRequestCommand(Guid.NewGuid());

        // Act
        using var response = await client.PostAsJsonAsync("/test-bad-request", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Assert failure logs - Should log Information level for BadRequest (< 500 status)
        var logs = logCollector.GetSnapshot();
        logs.Should().Contain(log => log.Level == LogLevel.Information &&
                                    log.Message.Contains("Bad request"));
    }

    [Fact]
    public async Task Query_ShouldReturnSuccessResultAsync()
    {
        // Arrange
        using var testServer = CreateTestServer(endpoints =>
        {
            endpoints.MapGet("/test-query", async (IMediator mediator, CancellationToken ct) =>
            {
                var query = new TestQuery(Guid.NewGuid());
                var result = await mediator.Send(query, ct);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            });
        });

        using var client = testServer.CreateClient();
        var logCollector = testServer.Services.GetRequiredService<FakeLogCollector>();

        // Act
        using var response = await client.GetAsync("/test-query");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify response content
        var jsonContent = await response.Content.ReadAsStringAsync();
        jsonContent.Should().Contain("Test Data");

        // Assert correlation and performance logs
        var logs = logCollector.GetSnapshot();
        logs.Should().Contain(log => log.Message.Contains("Beginning operation"));
        logs.Should().Contain(log => log.Message.Contains("Completed operation"));
    }

    [Fact]
    public void ServiceProvider_ShouldHaveApplicationServicesRegistered()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFakeLogging();

        var applicationAssembly = typeof(TestCommand).Assembly;
        var domainAssembly = typeof(Lewee.Domain.Entity).Assembly;

        services.AddApplication(applicationAssembly, domainAssembly);
        services.AddPipelineBehaviors();

        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert
        serviceProvider.GetService<IMediator>().Should().NotBeNull();
        serviceProvider.GetService<FluentValidation.IValidator<TestCommand>>().Should().NotBeNull();
        serviceProvider.GetService<FakeLogCollector>().Should().NotBeNull();
    }

    [Fact]
    public void ServiceProvider_ShouldResolveBehaviorsWithoutError()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFakeLogging();

        var applicationAssembly = typeof(TestCommand).Assembly;
        var domainAssembly = typeof(Lewee.Domain.Entity).Assembly;

        services.AddApplication(applicationAssembly, domainAssembly);
        services.AddPipelineBehaviors();

        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert
        var behaviors = serviceProvider.GetServices<IPipelineBehavior<TestCommand, CommandResult>>();
        behaviors.Should().NotBeEmpty();

        // Verify log collector is available for behavior tests
        var logCollector = serviceProvider.GetService<FakeLogCollector>();
        logCollector.Should().NotBeNull();
    }

    [Fact]
    public void ServiceProvider_ShouldResolveQueryHandlers()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFakeLogging();

        var applicationAssembly = typeof(TestCommand).Assembly;
        var domainAssembly = typeof(Lewee.Domain.Entity).Assembly;

        services.AddApplication(applicationAssembly, domainAssembly);

        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert
        var handler = serviceProvider.GetService<IRequestHandler<TestQuery, QueryResult<TestData>>>();
        handler.Should().NotBeNull();

        // Verify logging infrastructure is available
        var logCollector = serviceProvider.GetService<FakeLogCollector>();
        logCollector.Should().NotBeNull();
    }

    private static TestServer CreateTestServer(Action<IEndpointRouteBuilder> configureEndpoints)
    {
        var builder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddRouting(); // Required for UseRouting()
                services.AddFakeLogging();
                var applicationAssembly = typeof(TestCommand).Assembly;
                var domainAssembly = typeof(Lewee.Domain.Entity).Assembly;
                services.AddApplication(applicationAssembly, domainAssembly);
                services.AddPipelineBehaviors();
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(configureEndpoints);
            });

        return new TestServer(builder);
    }
}
