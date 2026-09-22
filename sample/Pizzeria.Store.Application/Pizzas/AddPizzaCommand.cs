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

public sealed record AddPizzaCommand(string Name, string? Description, decimal Price) : ICommand, ITenantRoleRequest
{
    public Guid TenantId => PizzaStore.Tenant.Id;

    public IReadOnlyCollection<string> Roles { get; } = [PizzaStore.Roles.StoreManagerCode];

    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "Used via mediation")]
    internal sealed class Validator : AbstractValidator<AddPizzaCommand>
    {
        public Validator()
        {
            this.RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(Pizza.FieldLengths.Name);

            this.RuleFor(x => x.Description)
                .MaximumLength(Pizza.FieldLengths.Description);

            this.RuleFor(x => x.Price)
                .GreaterThan(0);
        }
    }

    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "Used via mediation")]
    internal sealed class Handler : IRequestHandler<AddPizzaCommand, CommandResult>
    {
        private readonly IRepository<Pizza> repository;
        private readonly ILogger<Handler> logger;

        public Handler(IRepository<Pizza> repository, ILogger<Handler> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        public async Task<CommandResult> Handle(AddPizzaCommand request, CancellationToken cancellationToken)
        {
            var pizza = Pizza.Create(request.Name, request.Description ?? string.Empty, request.Price);

            await this.repository.AddAsync(pizza, cancellationToken);
            await this.repository.SaveChangesAsync(cancellationToken);

            this.logger.LogInformation("Pizza {PizzaId} added to the menu", pizza.Id);

            return CommandResult.Success();
        }
    }
}
