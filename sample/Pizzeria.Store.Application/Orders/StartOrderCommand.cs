using System.Diagnostics.CodeAnalysis;
using Correlate;
using Lewee.Application.Mediation.Requests;
using Lewee.Common;
using Lewee.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application.Orders;

public record StartOrderCommand : ICommand
{
    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "Used via mediation")]
    internal sealed class Handler : IRequestHandler<StartOrderCommand, CommandResult>
    {
        private readonly IRepository<Order> repository;
        private readonly IAuthenticatedUserService authenticatedUserService;
        private readonly ICorrelationContextAccessor correlationContextAccessor;
        private readonly ILogger<Handler> logger;

        public Handler(
            IRepository<Order> repository,
            IAuthenticatedUserService authenticatedUserService,
            ICorrelationContextAccessor correlationContextAccessor,
            ILogger<Handler> logger)
        {
            this.repository = repository;
            this.authenticatedUserService = authenticatedUserService;
            this.correlationContextAccessor = correlationContextAccessor;
            this.logger = logger;
        }

        public async Task<CommandResult> Handle(StartOrderCommand request, CancellationToken cancellationToken)
        {
            var cid = this.correlationContextAccessor.CorrelationContext?.CorrelationId;
            var correlationId = cid != null && Guid.TryParse(cid, out var parsed) ? parsed : Guid.NewGuid();
            var userId = this.authenticatedUserService.UserId ?? "Unknown";
            var order = Order.StartNewOrder(
                userId,
                correlationId);

            await this.repository.AddAsync(order, cancellationToken);
            await this.repository.SaveChangesAsync(cancellationToken);

            this.logger.LogOrderStarted(order.Id, userId);

            return CommandResult.Success();
        }
    }
}
