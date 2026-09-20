using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lewee.Application.Mediation.Requests;
using Lewee.Auth.Application;
using Lewee.Common;
using Lewee.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using Pizzeria.Common;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application.Pizzas;

public sealed record RemovePizzaCommand(Guid PizzaId) : ICommand, ITenantRoleRequest
{
    public Guid TenantId => PizzaStore.Tenant.Id;

    public IReadOnlyCollection<string> Roles { get; } = [PizzaStore.Roles.StoreManagerCode];

    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "Used via mediation")]
    internal sealed class Validator : AbstractValidator<RemovePizzaCommand>
    {
        public Validator()
        {
            this.RuleFor(x => x.PizzaId)
                .NotEmpty();
        }
    }

    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "Used via mediation")]
    internal sealed class Handler : IRequestHandler<RemovePizzaCommand, CommandResult>
    {
        private readonly IRepository<Pizza> repository;
        private readonly ILogger<Handler> logger;

        public Handler(IRepository<Pizza> repository, ILogger<Handler> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        public async Task<CommandResult> Handle(RemovePizzaCommand request, CancellationToken cancellationToken)
        {
            var pizza = await this.repository.RetrieveByIdAsync(request.PizzaId, cancellationToken);
            if (pizza is null)
            {
                return CommandResult.Fail(ResultStatus.NotFound, $"Pizza {request.PizzaId} not found");
            }

            pizza.RemoveFromMenu();

            await this.repository.SaveChangesAsync(cancellationToken);

            this.logger.LogInformation("Pizza {PizzaId} removed from the menu", request.PizzaId);

            return CommandResult.Success();
        }
    }
}
