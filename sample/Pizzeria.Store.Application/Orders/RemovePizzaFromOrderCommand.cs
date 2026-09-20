using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lewee.Application.Mediation.Requests;
using Lewee.Common;
using Lewee.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application.Orders;

public record RemovePizzaFromOrderCommand(Guid OrderId, Guid PizzaId) : ICommand
{
    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "Used via mediation")]
    internal sealed class Validator : AbstractValidator<RemovePizzaFromOrderCommand>
    {
        public Validator()
        {
            this.RuleFor(x => x.OrderId)
                .NotEmpty();

            this.RuleFor(x => x.PizzaId)
                .NotEmpty();
        }
    }

    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "Used via mediation")]
    internal sealed class Handler : IRequestHandler<RemovePizzaFromOrderCommand, CommandResult>
    {
        private readonly IRepository<Order> orderRepository;
        private readonly IRepository<Pizza> pizzaRepository;
        private readonly IAuthenticatedUserService authenticatedUserService;
        private readonly ILogger<Handler> logger;

        public Handler(
            IRepository<Order> orderRepository,
            IRepository<Pizza> pizzaRepository,
            IAuthenticatedUserService authenticatedUserService,
            ILogger<Handler> logger)
        {
            this.orderRepository = orderRepository;
            this.pizzaRepository = pizzaRepository;
            this.authenticatedUserService = authenticatedUserService;
            this.logger = logger;
        }

        public async Task<CommandResult> Handle(
            RemovePizzaFromOrderCommand request,
            CancellationToken cancellationToken)
        {
            var pizza = await this.pizzaRepository.RetrieveByIdAsync(request.PizzaId, cancellationToken);
            if (pizza is null)
            {
                return CommandResult.Fail(ResultStatus.NotFound, $"Pizza {request.PizzaId} not found");
            }

            var order = await this.orderRepository.QueryOneAsync(
                new GetOrderQuerySpec(request.OrderId),
                cancellationToken);
            if (order is null)
            {
                return CommandResult.Fail(ResultStatus.NotFound, $"Order {request.OrderId} not found");
            }

            if (!OrderOwnership.IsOwnedByCaller(order, this.authenticatedUserService))
            {
                return CommandResult.Fail(ResultStatus.Unauthorized, "Order does not belong to the caller.");
            }

            var result = order.RemovePizza(pizza);
            if (!result.IsSuccess)
            {
                return (CommandResult)result;
            }

            await this.orderRepository.SaveChangesAsync(cancellationToken);

            this.logger.LogInformation("Pizza {PizzaId} removed from order", request.PizzaId);

            return CommandResult.Success();
        }
    }
}
