using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Lewee.Application.Mediation.Requests;
using Lewee.Shared;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Lewee.Application.Tests.Unit;

/// <summary>
/// Tests for pipeline behaviors using TestHost
/// </summary>
public class PipelineBehaviorTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;
    private readonly HttpClient client;

    public PipelineBehaviorTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
        this.client = factory.CreateClient();
    }

    [Fact]
    public async Task ValidationBehavior_ValidCommand_ShouldPass()
    {
        // Arrange
        var command = new TestCommand("Valid Name", Guid.NewGuid());

        // Act
        var response = await client.PostAsJsonAsync("/test-command", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CommandResult>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ValidationBehavior_InvalidCommand_ShouldReturnBadRequest()
    {
        // Arrange
        var command = new TestCommand("", Guid.NewGuid()); // Empty name should fail validation

        // Act
        var response = await client.PostAsJsonAsync("/test-command", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<CommandResult>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage == "Name is required");
    }

    [Fact]
    public async Task DomainExceptionBehavior_DomainException_ShouldReturnBadRequest()
    {
        // Arrange
        var command = new TestDomainExceptionCommand(Guid.NewGuid());

        // Act
        var response = await client.PostAsJsonAsync("/test-domain-exception", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<CommandResult>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
        result.Errors.Should().Contain(e => e.ErrorMessage == "Test domain exception");
    }

    [Fact]
    public async Task UnhandledExceptionBehavior_UnhandledException_ShouldLogAndRethrow()
    {
        // Arrange
        var command = new TestUnhandledExceptionCommand(Guid.NewGuid());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await client.PostAsJsonAsync("/test-unhandled-exception", command);
        });

        exception.Message.Should().Be("Test unhandled exception");

        // Verify logging
        var logger = factory.GetLogger<Lewee.Application.Mediation.Behaviors.UnhandledExceptionBehavior<TestUnhandledExceptionCommand, CommandResult>>();
        logger.Collector.GetSnapshot().Should().Contain(log => 
            log.Level == LogLevel.Error && 
            log.Message.Contains("Unhandled Exception for Request TestUnhandledExceptionCommand"));
    }

    [Fact]
    public async Task CorrelationIdLoggingBehavior_ShouldAddCorrelationIdToLoggingScope()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var command = new TestCommand("Valid Name", correlationId);

        // Act
        await client.PostAsJsonAsync("/test-command", command);

        // Assert
        var logger = factory.GetLogger<Lewee.Application.Mediation.Behaviors.CorrelationIdLoggingBehavior<TestCommand, CommandResult>>();
        var logs = logger.Collector.GetSnapshot();
        
        // Verify that correlation ID was added to the logging scope
        logs.Should().Contain(log => log.Scopes.Any(scope => 
            scope.ContainsKey(LoggingConsts.CorrelationId) && 
            scope[LoggingConsts.CorrelationId].Equals(correlationId)));
    }

    [Fact]
    public async Task TenantLoggingBehavior_TenantRequest_ShouldAddTenantIdToLoggingScope()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var command = new TestTenantCommand(tenantId, "Valid Name", correlationId);

        // Act
        await client.PostAsJsonAsync("/test-tenant-command", command);

        // Assert
        var logger = factory.GetLogger<Lewee.Application.Mediation.Behaviors.TenantLoggingBehavior<TestTenantCommand, CommandResult>>();
        var logs = logger.Collector.GetSnapshot();
        
        // Verify that tenant ID was added to the logging scope
        logs.Should().Contain(log => log.Scopes.Any(scope => 
            scope.ContainsKey(LoggingConsts.TenantId) && 
            scope[LoggingConsts.TenantId].Equals(tenantId)));
    }

    [Fact]
    public async Task PerformanceBehavior_ShouldLogTimedOperation()
    {
        // Arrange
        var command = new TestCommand("Valid Name", Guid.NewGuid());

        // Act
        await client.PostAsJsonAsync("/test-command", command);

        // Assert
        var logger = factory.GetLogger<Lewee.Application.Mediation.Behaviors.PerformanceBehavior<TestCommand, CommandResult>>();
        var logs = logger.Collector.GetSnapshot();
        
        // Verify that timed operation was logged
        logs.Should().Contain(log => 
            log.Message.Contains("TestCommand Handler") &&
            log.Message.Contains("ms"));
    }

    [Fact]
    public async Task Query_ShouldReturnSuccessResult()
    {
        // Act
        var response = await client.GetAsync("/test-query");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<QueryResult<TestData>>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Value.Should().Be("Test Data");
    }

    [Fact]
    public async Task FailureLoggingBehavior_ServerError_ShouldLogError()
    {
        // Arrange
        var command = new TestServerErrorCommand(Guid.NewGuid());

        // Act
        var response = await client.PostAsJsonAsync("/test-server-error", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        // Verify error logging
        var logger = factory.GetLogger<Lewee.Application.Mediation.Behaviors.FailureLoggingBehavior<TestServerErrorCommand, CommandResult>>();
        var logs = logger.Collector.GetSnapshot();
        logs.Should().Contain(log => 
            log.Level == LogLevel.Error && 
            log.Message.Contains("Unexpected error occurred"));
    }

    [Fact]
    public async Task FailureLoggingBehavior_BadRequest_ShouldLogInformation()
    {
        // Arrange
        var command = new TestBadRequestCommand(Guid.NewGuid());

        // Act
        var response = await client.PostAsJsonAsync("/test-bad-request", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Verify information logging
        var logger = factory.GetLogger<Lewee.Application.Mediation.Behaviors.FailureLoggingBehavior<TestBadRequestCommand, CommandResult>>();
        var logs = logger.Collector.GetSnapshot();
        logs.Should().Contain(log => 
            log.Level == LogLevel.Information && 
            log.Message.Contains("Bad request"));
    }
}