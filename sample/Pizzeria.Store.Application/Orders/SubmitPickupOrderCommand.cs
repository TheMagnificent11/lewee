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

public record SubmitPickupOrderCommand(Guid OrderId) : ICommand
{
    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "Used via mediation")]
    internal sealed class Validator : AbstractValidator<SubmitPickupOrderCommand>
    {
        public Validator()
        {
            this.RuleFor(x => x.OrderId)
                .NotEmpty();
        }
    }

    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "Used via mediation")]
    internal sealed class Handler : IRequestHandler<SubmitPickupOrderCommand, CommandResult>
    {
        private readonly IRepository<Order> orderRepository;
        private readonly IAuthenticatedUserService authenticatedUserService;
        private readonly ICorrelationContextAccessor correlationContextAccessor;
        private readonly ILogger<Handler> logger;

        public Handler(
            IRepository<Order> orderRepository,
            IAuthenticatedUserService authenticatedUserService,
            ICorrelationContextAccessor correlationContextAccessor,
            ILogger<Handler> logger)
        {
            this.orderRepository = orderRepository;
            this.authenticatedUserService = authenticatedUserService;
            this.correlationContextAccessor = correlationContextAccessor;
            this.logger = logger;
        }

        public async Task<CommandResult> Handle(
            SubmitPickupOrderCommand request,
            CancellationToken cancellationToken)
        {
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

            var correlationId = this.correlationContextAccessor.GetCorrelationId();

            var result = order.SubmitPickupOrder(correlationId);
            if (!result.IsSuccess)
            {
                return (CommandResult)result;
            }

            await this.orderRepository.SaveChangesAsync(cancellationToken);

            this.logger.LogInformation("Order {OrderId} submitted for pickup", request.OrderId);

            return CommandResult.Success();
        }
    }
}
