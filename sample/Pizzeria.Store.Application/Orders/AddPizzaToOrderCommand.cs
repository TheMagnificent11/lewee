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
        private readonly IRepository<Order> repository;
        private readonly ILogger<Handler> logger;
        public Handler(IRepository<Order> repository, ILogger<Handler> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        public async Task<CommandResult> Handle(AddPizzaToOrderCommand request, CancellationToken cancellationToken)
        {
            using (this.logger.BeginScope(new Dictionary<string, object>
            {
                { nameof(request.CorrelationId), request.CorrelationId }
            }))
            {
                var pizza = Menu.Pizzas.FirstOrDefault(x => x.Id == request.PizzaId);
                if (pizza is null)
                {
                    return CommandResult.Fail(ResultStatus.NotFound, $"Pizza {request.PizzaId} not found");
                }

                var order = await this.repository.QueryOneAsync(
                    new GetOrderQuerySpec(request.OrderId),
                    cancellationToken);
                if (order is null)
                {
                    return CommandResult.Fail(ResultStatus.NotFound, $"Order {request.OrderId} not found");
                }

                order.AddPizza(pizza);

                await this.repository.SaveChangesAsync(cancellationToken);

                this.logger.LogInformation("Pizza {PizzaId} added to order", request.PizzaId);

                return CommandResult.Success();
            }
        }
    }
}
