using FluentValidation;
using Lewee.Application.Mediation.Requests;
using Lewee.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using Pizzeria.Auth;
using Pizzeria.Common;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application.Customers;

public record CreateCustomerCommand(string Username, string Password, Guid CorrelationId) : ICommand
{
    internal class Validator : AbstractValidator<CreateCustomerCommand>
    {
        public Validator()
        {
            this.RuleFor(x => x.Username)
                .NotEmpty()
                .MaximumLength(100);

            this.RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(100);
        }
    }

    internal sealed class Handler : IRequestHandler<CreateCustomerCommand, CommandResult>
    {
        private readonly IRepository<User> repository;
        private readonly IAuthServerAdminClient authServerAdminClient;
        private readonly ILogger<Handler> logger;

        public Handler(
            IRepository<User> repository,
            IAuthServerAdminClient authServerAdminClient,
            ILogger<Handler> logger)
        {
            this.repository = repository;
            this.authServerAdminClient = authServerAdminClient;
            this.logger = logger;
        }

        public async Task<CommandResult> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            // Create user in Keycloak
            await this.authServerAdminClient.CreateUserAsync(
                Environments.Auth.RealmName,
                request.Username,
                request.Password,
                cancellationToken);

            // Get the Keycloak user ID
            var keycloakUserId = await this.authServerAdminClient.GetUserIdAsync(
                Environments.Auth.RealmName,
                request.Username,
                cancellationToken);

            // Create user entity with Keycloak user ID
            var user = User.Create(keycloakUserId, request.CorrelationId);

            await this.repository.AddAsync(user, cancellationToken);
            await this.repository.SaveChangesAsync(cancellationToken);

            this.logger.LogCustomerCreated(
                user.Id,
                keycloakUserId);

            return CommandResult.Success();
        }
    }
}
