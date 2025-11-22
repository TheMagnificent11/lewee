using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lewee.Application.Mediation.Requests;
using Lewee.Domain;
using MediatR;

namespace Lewee.Application.Tests.Unit;

[SuppressMessage(
    "StyleCop.CSharp.DocumentationRules",
    "SA1649:File name should match first type name",
    Justification = "Test models file contains multiple related test classes for testing purposes")]
internal sealed record TestCommand(string Name, Guid CorrelationId) : ICommand
{
    internal sealed class Validator : AbstractValidator<TestCommand>
    {
        public Validator()
        {
            this.RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        }
    }

    internal sealed class Handler : IRequestHandler<TestCommand, CommandResult>
    {
        public Task<CommandResult> Handle(TestCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(CommandResult.Success());
        }
    }
}

[SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1402:File may only contain a single type",
    Justification = "Test model classes are grouped together for easier test maintenance")]
internal sealed record TestDomainExceptionCommand(Guid CorrelationId) : ICommand
{
    internal sealed class Handler : IRequestHandler<TestDomainExceptionCommand, CommandResult>
    {
        public Task<CommandResult> Handle(TestDomainExceptionCommand request, CancellationToken cancellationToken)
        {
            throw new DomainException("Test domain exception");
        }
    }
}

[SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1402:File may only contain a single type",
    Justification = "Test model classes are grouped together for easier test maintenance")]
internal sealed record TestUnhandledExceptionCommand(Guid CorrelationId) : ICommand
{
    internal sealed class Handler : IRequestHandler<TestUnhandledExceptionCommand, CommandResult>
    {
        public Task<CommandResult> Handle(TestUnhandledExceptionCommand request, CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("Test unhandled exception");
        }
    }
}

[SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1402:File may only contain a single type",
    Justification = "Test model classes are grouped together for easier test maintenance")]
internal sealed record TestQuery(Guid CorrelationId) : IQuery<TestData>
{
    internal sealed class Handler : IRequestHandler<TestQuery, QueryResult<TestData>>
    {
        public Task<QueryResult<TestData>> Handle(TestQuery request, CancellationToken cancellationToken)
        {
            var data = new TestData("Test Data");
            return Task.FromResult(QueryResult<TestData>.Success(data));
        }
    }
}

[SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1402:File may only contain a single type",
    Justification = "Test model classes are grouped together for easier test maintenance")]
internal sealed record TestData(string Value);

[SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1402:File may only contain a single type",
    Justification = "Test model classes are grouped together for easier test maintenance")]
internal sealed record TestTenantCommand(Guid TenantId, string Name, Guid CorrelationId) : ICommand, ITenantRequest
{
    internal sealed class Handler : IRequestHandler<TestTenantCommand, CommandResult>
    {
        public Task<CommandResult> Handle(TestTenantCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(CommandResult.Success());
        }
    }
}

[SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1402:File may only contain a single type",
    Justification = "Test model classes are grouped together for easier test maintenance")]
internal sealed record TestServerErrorCommand(Guid CorrelationId) : ICommand
{
    internal sealed class Handler : IRequestHandler<TestServerErrorCommand, CommandResult>
    {
        public Task<CommandResult> Handle(TestServerErrorCommand request, CancellationToken cancellationToken)
        {
            // Use a status that maps to >= 500 in the behavior check
            var customStatus = (ResultStatus)500; // This will be treated as >= 500 in FailureLoggingBehavior
            return Task.FromResult(CommandResult.Fail(customStatus, "Server error occurred"));
        }
    }
}

[SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1402:File may only contain a single type",
    Justification = "Test model classes are grouped together for easier test maintenance")]
internal sealed record TestBadRequestCommand(Guid CorrelationId) : ICommand
{
    internal sealed class Handler : IRequestHandler<TestBadRequestCommand, CommandResult>
    {
        public Task<CommandResult> Handle(TestBadRequestCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(CommandResult.Fail(ResultStatus.BadRequest, "Bad request error"));
        }
    }
}
