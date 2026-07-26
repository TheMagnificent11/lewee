using System.Diagnostics.CodeAnalysis;
using Correlate;
using FluentValidation;
using Lewee.Application.Mediation.Requests;
using Lewee.Common;
using Lewee.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application.Customers;

public record CreateCustomerCommand(string ExternalUserId) : ICommand
{
    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "Used via mediation")]
    internal sealed class Validator : AbstractValidator<CreateCustomerCommand>
    {
        public Validator()
        {
            this.RuleFor(x => x.ExternalUserId)
                .NotEmpty()
                .MaximumLength(User.FieldLengths.ExternalId);
        }
    }

    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "Used via mediation")]
    internal sealed class Handler : IRequestHandler<CreateCustomerCommand, CommandResult>
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

        public async Task<CommandResult> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            // Check if user already exists
            var specification = new UserByExternalIdSpecification(request.ExternalUserId);
            var existingUser = await this.repository.QueryOneAsync(specification, cancellationToken);

            if (existingUser != null)
            {
                // User already exists, return success
                this.logger.LogCustomerAlreadyExists(existingUser.Id, request.ExternalUserId);
                return CommandResult.Success();
            }

            var cid = this.correlationContextAccessor.CorrelationContext?.CorrelationId;
            var correlationId = cid != null && Guid.TryParse(cid, out var parsed) ? parsed : Guid.NewGuid();

            // Create user entity with Keycloak user ID
            var user = User.Create(request.ExternalUserId, correlationId);

            await this.repository.AddAsync(user, cancellationToken);
            await this.repository.SaveChangesAsync(cancellationToken);

            this.logger.LogCustomerCreated(
                user.Id,
                request.ExternalUserId);

            return CommandResult.Success();
        }
    }
}
