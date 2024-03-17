using Lewee.Application.Mediation.Requests;
using Lewee.Domain;
using MediatR;
using Sample.Pizzeria.Domain;
using Serilog;

namespace Sample.Pizzeria.Application;

public sealed class StartOrderCommand : ICommand
{
    public StartOrderCommand(string userId, Guid correlationId)
    {
        this.UserId = userId;
        this.CorrelationId = correlationId;
    }

    public string UserId { get; }
    public Guid CorrelationId { get; }

    internal class StartOrderCommandHandler : IRequestHandler<StartOrderCommand, CommandResult>
    {
        private readonly IRepository<Order> repository;
        private readonly ILogger logger;

        public StartOrderCommandHandler(IRepository<Order> repository, ILogger logger)
        {
            this.repository = repository;
            this.logger = logger.ForContext<StartOrderCommandHandler>();
        }

        public async Task<CommandResult> Handle(StartOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order(request.UserId, request.CorrelationId);

            this.repository.Add(order);

            await this.repository.SaveChanges(cancellationToken);

            this.logger.Information("Order {OrderId} started for User {UserId}", order.Id, order.UserId);

            return CommandResult.Success();
        }
    }
}
