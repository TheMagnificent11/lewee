using FluentValidation;
using FreeMediator;
using Lewee.Application.Mediation.Requests;
using Lewee.Domain;
using Microsoft.Extensions.Logging;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application.Orders;

public record AddPizzaToOrderCommand(Guid OrderId, Guid PizzaId, Guid CorrelationId) : ICommand
{
    internal class Validator : AbstractValidator<AddPizzaToOrderCommand>
    {
        public Validator()
        {
            this.RuleFor(x => x.OrderId)
                .NotEmpty();

            this.RuleFor(x => x.PizzaId)
                .NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<AddPizzaToOrderCommand, CommandResult>
    {
        private readonly IRepository<Order> orderRepository;
        private readonly IRepository<Pizza> pizzaRepository;
        private readonly ILogger<Handler> logger;

        public Handler(
            IRepository<Order> orderRepository,
            IRepository<Pizza> pizzaRepository,
            ILogger<Handler> logger)
        {
            this.orderRepository = orderRepository;
            this.pizzaRepository = pizzaRepository;
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

            order.AddPizza(pizza);

            await this.orderRepository.SaveChangesAsync(cancellationToken);

            this.logger.LogInformation("Pizza {PizzaId} added to order", request.PizzaId);

            return CommandResult.Success();
        }
    }
}
