using System.Diagnostics.CodeAnalysis;
using Correlate;
using FluentValidation;
using Lewee.Application.Mediation.Behaviors;
using Lewee.Application.Mediation.Requests;
using Lewee.Auth.Domain;
using Lewee.Common;
using Lewee.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lewee.Auth.Application;

/// <summary>
/// Creates a user for an external identity.
/// </summary>
public sealed record CreateUserCommand(string ExternalUserId) : ICommand
{
    [SuppressMessage(
        "Performance",
        "CA1812:Avoid uninstantiated internal classes",
        Justification = "Used via mediation")]
    internal sealed class Validator : AbstractValidator<CreateUserCommand>
    {
        public Validator()
        {
            this.RuleFor(command => command.ExternalUserId)
                .NotEmpty()
                .MaximumLength(User.FieldLengths.ExternalId);
        }
    }

    [SuppressMessage(
        "Performance",
        "CA1812:Avoid uninstantiated internal classes",
        Justification = "Used via mediation")]
    internal sealed class Handler : IRequestHandler<CreateUserCommand, CommandResult>
    {
        private readonly IRepository<User> repository;
        private readonly ICorrelationContextAccessor correlationContextAccessor;
        private readonly ILogger<Handler> logger;

        public Handler(
            IRepository<User> repository,
            ICorrelationContextAccessor correlationContextAccessor,
            ILogger<Handler> logger)
        {
            this.repository = repository;
            this.correlationContextAccessor = correlationContextAccessor;
            this.logger = logger;
        }

        public async Task<CommandResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var specification = new UserByExternalIdSpecification(request.ExternalUserId);
            var existingUser = await this.repository.QueryOneAsync(specification, cancellationToken);

            if (existingUser != null)
            {
                this.logger.LogUserAlreadyExists(existingUser.Id, request.ExternalUserId);
                return CommandResult.Success();
            }

            var user = User.Create(
                request.ExternalUserId,
                this.correlationContextAccessor.GetCorrelationId());

            await this.repository.AddAsync(user, cancellationToken);
            await this.repository.SaveChangesAsync(cancellationToken);

            this.logger.LogUserCreated(user.Id, request.ExternalUserId);
            return CommandResult.Success();
        }
    }
}
