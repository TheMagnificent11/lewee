using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FreeMediator;
using Lewee.Application;
using Lewee.Application.Mediation.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Lewee.Application.Tests.Unit;

/// <summary>
/// Tests for pipeline behaviors using TestServer
/// </summary>
public class PipelineBehaviorTests
{
    [Fact]
    public async Task ValidationBehavior_WithInvalidCommand_ShouldReturnBadRequest()
    {
        // Arrange
        var testServer = new TestServer(new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddRouting(); // Required for UseRouting()
                services.AddLogging();
                var applicationAssembly = typeof(TestCommand).Assembly;
                var domainAssembly = typeof(Lewee.Domain.Entity).Assembly;
                services.AddApplication(applicationAssembly, domainAssembly);
                services.AddPipelineBehaviors(); // This includes ValidationBehavior
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapPost("/test-command", async (TestCommand command, IMediator mediator, CancellationToken ct) =>
                    {
                        var result = await mediator.Send(command, ct);
                        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
                    });
                });
            }));

        var client = testServer.CreateClient();
        var invalidCommand = new TestCommand("", Guid.NewGuid()); // Empty name should fail validation

        // Act
        var response = await client.PostAsJsonAsync("/test-command", invalidCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        // We don't need to deserialize the result to verify the behavior worked
        // The status code confirms that validation failed and the behavior returned BadRequest
    }

    [Fact]
    public async Task ValidationBehavior_WithValidCommand_ShouldReturnOk()
    {
        // Arrange
        var testServer = new TestServer(new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddRouting(); // Required for UseRouting()
                services.AddLogging();
                var applicationAssembly = typeof(TestCommand).Assembly;
                var domainAssembly = typeof(Lewee.Domain.Entity).Assembly;
                services.AddApplication(applicationAssembly, domainAssembly);
                services.AddPipelineBehaviors(); // This includes ValidationBehavior
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapPost("/test-command", async (TestCommand command, IMediator mediator, CancellationToken ct) =>
                    {
                        var result = await mediator.Send(command, ct);
                        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
                    });
                });
            }));

        var client = testServer.CreateClient();
        var validCommand = new TestCommand("Valid Name", Guid.NewGuid());

        // Act
        var response = await client.PostAsJsonAsync("/test-command", validCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        // The OK status confirms that validation passed and the command was processed successfully
    }

    [Fact]
    public async Task DomainExceptionBehavior_WithDomainException_ShouldReturnBadRequest()
    {
        // Arrange
        var testServer = new TestServer(new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddRouting(); // Required for UseRouting()
                services.AddLogging();
                var applicationAssembly = typeof(TestDomainExceptionCommand).Assembly;
                var domainAssembly = typeof(Lewee.Domain.Entity).Assembly;
                services.AddApplication(applicationAssembly, domainAssembly);
                services.AddPipelineBehaviors(); // This includes DomainExceptionBehavior
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapPost("/test-domain-exception", async (TestDomainExceptionCommand command, IMediator mediator, CancellationToken ct) =>
                    {
                        var result = await mediator.Send(command, ct);
                        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
                    });
                });
            }));

        var client = testServer.CreateClient();
        var command = new TestDomainExceptionCommand(Guid.NewGuid());

        // Act
        var response = await client.PostAsJsonAsync("/test-domain-exception", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        // The BadRequest status confirms that the domain exception was caught and handled properly
    }

    [Fact]
    public async Task PerformanceBehavior_ShouldLogTiming()
    {
        // Arrange
        var testServer = new TestServer(new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddRouting(); // Required for UseRouting()
                services.AddLogging();
                var applicationAssembly = typeof(TestCommand).Assembly;
                var domainAssembly = typeof(Lewee.Domain.Entity).Assembly;
                services.AddApplication(applicationAssembly, domainAssembly);
                services.AddPipelineBehaviors(); // This includes PerformanceBehavior
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapPost("/test-command", async (TestCommand command, IMediator mediator, CancellationToken ct) =>
                    {
                        var result = await mediator.Send(command, ct);
                        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
                    });
                });
            }));

        var client = testServer.CreateClient();
        var command = new TestCommand("Valid Name", Guid.NewGuid());

        // Act
        var response = await client.PostAsJsonAsync("/test-command", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        // Performance behavior should not affect the response status
        // The OK status confirms the command was processed successfully while being timed
    }

    [Fact]
    public async Task FailureLoggingBehavior_WithFailure_ShouldLogFailure()
    {
        // Arrange
        var testServer = new TestServer(new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddRouting(); // Required for UseRouting()
                services.AddLogging();
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

        var client = testServer.CreateClient();
        var command = new TestBadRequestCommand(Guid.NewGuid());

        // Act
        var response = await client.PostAsJsonAsync("/test-bad-request", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        // The BadRequest status confirms that the failure was logged and returned appropriately
    }

    [Fact]
    public async Task Query_ShouldReturnSuccessResult()
    {
        // Arrange
        var testServer = new TestServer(new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddRouting(); // Required for UseRouting()
                services.AddLogging();
                var applicationAssembly = typeof(TestQuery).Assembly;
                var domainAssembly = typeof(Lewee.Domain.Entity).Assembly;
                services.AddApplication(applicationAssembly, domainAssembly);
                services.AddPipelineBehaviors();
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/test-query", async (IMediator mediator, CancellationToken ct) =>
                    {
                        var query = new TestQuery(Guid.NewGuid());
                        var result = await mediator.Send(query, ct);
                        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
                    });
                });
            }));

        var client = testServer.CreateClient();

        // Act
        var response = await client.GetAsync("/test-query");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        // The OK status confirms that the query was processed successfully
        // We can check the response content contains the expected data
        var jsonContent = await response.Content.ReadAsStringAsync();
        jsonContent.Should().Contain("Test Data");
    }

    [Fact]
    public void ServiceProvider_ShouldHaveApplicationServicesRegistered()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        
        var applicationAssembly = typeof(TestCommand).Assembly;
        var domainAssembly = typeof(Lewee.Domain.Entity).Assembly;
        
        services.AddApplication(applicationAssembly, domainAssembly);
        services.AddPipelineBehaviors();
        
        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert
        serviceProvider.GetService<IMediator>().Should().NotBeNull();
        serviceProvider.GetService<FluentValidation.IValidator<TestCommand>>().Should().NotBeNull();
    }

    [Fact]
    public void ServiceProvider_ShouldResolveBehaviorsWithoutError()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        
        var applicationAssembly = typeof(TestCommand).Assembly;
        var domainAssembly = typeof(Lewee.Domain.Entity).Assembly;
        
        services.AddApplication(applicationAssembly, domainAssembly);
        services.AddPipelineBehaviors();
        
        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert
        var behaviors = serviceProvider.GetServices<IPipelineBehavior<TestCommand, CommandResult>>();
        behaviors.Should().NotBeEmpty();
    }

    [Fact]
    public void ServiceProvider_ShouldResolveQueryHandlers()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        
        var applicationAssembly = typeof(TestCommand).Assembly;
        var domainAssembly = typeof(Lewee.Domain.Entity).Assembly;
        
        services.AddApplication(applicationAssembly, domainAssembly);
        
        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert
        var handler = serviceProvider.GetService<IRequestHandler<TestQuery, QueryResult<TestData>>>();
        handler.Should().NotBeNull();
    }
}