using System.Diagnostics.CodeAnalysis;
using Correlate;
using FluentValidation;
using Lewee.Application.Mediation.Behaviors;
using Lewee.Application.Mediation.Requests;
using Lewee.Common;
using Lewee.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application.Orders;

public record AddPizzaToOrderCommand(Guid OrderId, Guid PizzaId) : ICommand, IOrderOwnerRequest
{
    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "Used via mediation")]
    internal sealed class Validator : AbstractValidator<AddPizzaToOrderCommand>
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
    internal sealed class Handler : IRequestHandler<AddPizzaToOrderCommand, CommandResult>
    {
        private readonly IRepository<Order> orderRepository;
        private readonly IRepository<Pizza> pizzaRepository;
        private readonly ICorrelationContextAccessor correlationContextAccessor;
        private readonly ILogger<Handler> logger;

        public Handler(
            IRepository<Order> orderRepository,
            IRepository<Pizza> pizzaRepository,
            ICorrelationContextAccessor correlationContextAccessor,
            ILogger<Handler> logger)
        {
            this.orderRepository = orderRepository;
            this.pizzaRepository = pizzaRepository;
            this.correlationContextAccessor = correlationContextAccessor;
            this.logger = logger;
        }

        public async Task<CommandResult> Handle(AddPizzaToOrderCommand request, CancellationToken cancellationToken)
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

            var correlationId = this.correlationContextAccessor.GetCorrelationId();

            if (!pizza.IsAvailable)
            {
                return CommandResult.Fail(ResultStatus.BadRequest, "Pizza is no longer available.");
            }

            var result = order.AddPizza(pizza, correlationId);
            if (!result.IsSuccess)
            {
                return (CommandResult)result;
            }

            await this.orderRepository.SaveChangesAsync(cancellationToken);

            this.logger.LogInformation("Pizza {PizzaId} added to order", request.PizzaId);

            return CommandResult.Success();
        }
    }
}
