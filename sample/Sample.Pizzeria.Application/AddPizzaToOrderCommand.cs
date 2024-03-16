using Lewee.Application.Mediation.Requests;
using Lewee.Domain;
using MediatR;
using Sample.Pizzeria.Domain;
using Serilog;

namespace Sample.Pizzeria.Application;

public sealed class AddPizzaToOrderCommand : ICommand
{
    public AddPizzaToOrderCommand(Guid orderId, int pizzaId, Guid correlationId)
    {
        this.OrderId = orderId;
        this.PizzaId = pizzaId;
        this.CorrelationId = correlationId;
    }

    public Guid OrderId { get; }
    public int PizzaId { get; }
    public Guid CorrelationId { get; }

    internal class AddPizzaToOrderCommandHandler : IRequestHandler<AddPizzaToOrderCommand, CommandResult>
    {
        private readonly IRepository<Order> orderRepository;
        private readonly ILogger logger;

        public AddPizzaToOrderCommandHandler(IRepository<Order> orderRepository, ILogger logger)
        {
            this.orderRepository = orderRepository;
            this.logger = logger.ForContext<AddPizzaToOrderCommandHandler>();
        }

        public async Task<CommandResult> Handle(AddPizzaToOrderCommand request, CancellationToken cancellationToken)
        {
            var pizza = Menu.Pizzas.FirstOrDefault(x => x.Id == request.PizzaId);
            if (pizza == null)
            {
                this.logger.Warning("Invalid Pizza ID {PizzaId}", request.PizzaId);
                return CommandResult.Fail(ResultStatus.BadRequest, "Pizza not found on menu");
            }

            var order = await this.orderRepository.RetrieveById(request.OrderId, cancellationToken);
            if (order == null)
            {
                this.logger.Warning("Invalid Order ID {OrderId}", request.OrderId);
                return CommandResult.Fail(ResultStatus.NotFound, "Order not found");
            }

            order.AddPizza(pizza, request.CorrelationId);

            await this.orderRepository.SaveChanges(cancellationToken);

            this.logger.Information("Pizza {PizzaId} was added to Order {OrderId}", request.PizzaId, request.OrderId);

            return CommandResult.Success();
        }
    }
}
