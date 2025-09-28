using FluentValidation;
using FreeMediator;
using Lewee.Application.Mediation.Requests;
using Lewee.Domain;

namespace Lewee.Application.Tests.Unit;

/// <summary>
/// Test command for testing pipeline behaviors
/// </summary>
public record TestCommand(string Name, Guid CorrelationId) : ICommand
{
    public class Validator : AbstractValidator<TestCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        }
    }

    public class Handler : IRequestHandler<TestCommand, CommandResult>
    {
        public Task<CommandResult> Handle(TestCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(CommandResult.Success());
        }
    }
}

/// <summary>
/// Test command that throws a domain exception
/// </summary>
public record TestDomainExceptionCommand(Guid CorrelationId) : ICommand
{
    public class Handler : IRequestHandler<TestDomainExceptionCommand, CommandResult>
    {
        public Task<CommandResult> Handle(TestDomainExceptionCommand request, CancellationToken cancellationToken)
        {
            throw new DomainException("Test domain exception");
        }
    }
}

/// <summary>
/// Test command that throws an unhandled exception
/// </summary>
public record TestUnhandledExceptionCommand(Guid CorrelationId) : ICommand
{
    public class Handler : IRequestHandler<TestUnhandledExceptionCommand, CommandResult>
    {
        public Task<CommandResult> Handle(TestUnhandledExceptionCommand request, CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("Test unhandled exception");
        }
    }
}

/// <summary>
/// Test query for testing pipeline behaviors
/// </summary>
public record TestQuery(Guid CorrelationId) : IQuery<TestData>
{
    public class Handler : IRequestHandler<TestQuery, QueryResult<TestData>>
    {
        public Task<QueryResult<TestData>> Handle(TestQuery request, CancellationToken cancellationToken)
        {
            var data = new TestData("Test Data");
            return Task.FromResult(QueryResult<TestData>.Success(data));
        }
    }
}

/// <summary>
/// Test data class for query responses
/// </summary>
public record TestData(string Value);

/// <summary>
/// Test tenant command for testing tenant-specific behaviors
/// </summary>
public record TestTenantCommand(Guid TenantId, string Name, Guid CorrelationId) : ICommand, ITenantRequest
{
    public class Handler : IRequestHandler<TestTenantCommand, CommandResult>
    {
        public Task<CommandResult> Handle(TestTenantCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(CommandResult.Success());
        }
    }
}

/// <summary>
/// Test command that returns a server error for testing failure logging
/// </summary>
public record TestServerErrorCommand(Guid CorrelationId) : ICommand
{
    public class Handler : IRequestHandler<TestServerErrorCommand, CommandResult>
    {
        public Task<CommandResult> Handle(TestServerErrorCommand request, CancellationToken cancellationToken)
        {
            // Use a status that maps to >= 500 in the behavior check
            var customStatus = (ResultStatus)500; // This will be treated as >= 500 in FailureLoggingBehavior
            return Task.FromResult(CommandResult.Fail(customStatus, "Server error occurred"));
        }
    }
}

/// <summary>
/// Test command that returns a bad request for testing failure logging
/// </summary>
public record TestBadRequestCommand(Guid CorrelationId) : ICommand
{
    public class Handler : IRequestHandler<TestBadRequestCommand, CommandResult>
    {
        public Task<CommandResult> Handle(TestBadRequestCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(CommandResult.Fail(ResultStatus.BadRequest, "Bad request error"));
        }
    }
}