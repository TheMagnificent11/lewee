using Lewee.Application.Mediation.Requests;
using Lewee.Domain;
using MediatR;
using Sample.Pizzeria.Domain;
using Serilog;

namespace Sample.Pizzeria.Application;

public sealed class RemovePizzaFromOrderCommand : ICommand
{
    public RemovePizzaFromOrderCommand(Guid orderId, int pizzaId, Guid correlationId)
    {
        this.OrderId = orderId;
        this.PizzaId = pizzaId;
        this.CorrelationId = correlationId;
    }

    public Guid OrderId { get; }
    public int PizzaId { get; }
    public Guid CorrelationId { get; }

    internal class RemovePizzaForOrderCommand : IRequestHandler<RemovePizzaFromOrderCommand, CommandResult>
    {
        private readonly IRepository<Order> repository;
        private readonly ILogger logger;

        public RemovePizzaForOrderCommand(IRepository<Order> repository, ILogger logger)
        {
            this.repository = repository;
            this.logger = logger.ForContext<RemovePizzaForOrderCommand>();
        }

        public async Task<CommandResult> Handle(RemovePizzaFromOrderCommand request, CancellationToken cancellationToken)
        {
            var pizza = Menu.Pizzas.FirstOrDefault(x => x.Id == request.PizzaId);
            if (pizza == null)
            {
                this.logger.Warning("Invalid Pizza ID {PizzaId}", request.PizzaId);
                return CommandResult.Fail(ResultStatus.BadRequest, "Pizza not found on menu");
            }

            var order = await this.repository.RetrieveById(request.OrderId, cancellationToken);
            if (order == null)
            {
                this.logger.Warning("Invalid Order ID {OrderId}", request.OrderId);
                return CommandResult.Fail(ResultStatus.NotFound, "Order not found");
            }

            order.RemovePizza(pizza, request.CorrelationId);

            await this.repository.SaveChanges(cancellationToken);

            this.logger.Information(
                "Pizza {PizzaId} was removed from Order {OrderId}",
                request.PizzaId,
                request.OrderId);

            return CommandResult.Success();
        }
    }
}
