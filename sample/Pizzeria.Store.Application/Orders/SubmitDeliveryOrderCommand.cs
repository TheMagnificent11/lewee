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

public record SubmitDeliveryOrderCommand(Guid OrderId, string DeliveryAddress) : ICommand, IOrderOwnerRequest
{
    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "Used via mediation")]
    internal sealed class Validator : AbstractValidator<SubmitDeliveryOrderCommand>
    {
        public Validator()
        {
            this.RuleFor(x => x.OrderId)
                .NotEmpty();

            this.RuleFor(x => x.DeliveryAddress)
                .NotEmpty()
                .MaximumLength(Order.FieldLengths.DeliveryAddress);
        }
    }

    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "Used via mediation")]
    internal sealed class Handler : IRequestHandler<SubmitDeliveryOrderCommand, CommandResult>
    {
        private readonly IRepository<Order> orderRepository;
        private readonly ICorrelationContextAccessor correlationContextAccessor;
        private readonly ILogger<Handler> logger;

        public Handler(
            IRepository<Order> orderRepository,
            ICorrelationContextAccessor correlationContextAccessor,
            ILogger<Handler> logger)
        {
            this.orderRepository = orderRepository;
            this.correlationContextAccessor = correlationContextAccessor;
            this.logger = logger;
        }

        public async Task<CommandResult> Handle(
            SubmitDeliveryOrderCommand request,
            CancellationToken cancellationToken)
        {
            var order = await this.orderRepository.QueryOneAsync(
                new GetOrderQuerySpec(request.OrderId),
                cancellationToken);
            if (order is null)
            {
                return CommandResult.Fail(ResultStatus.NotFound, $"Order {request.OrderId} not found");
            }

            var correlationId = this.correlationContextAccessor.GetCorrelationId();

            var result = order.SubmitDeliveryOrder(request.DeliveryAddress, correlationId);
            if (!result.IsSuccess)
            {
                return (CommandResult)result;
            }

            await this.orderRepository.SaveChangesAsync(cancellationToken);

            this.logger.LogInformation("Order {OrderId} submitted for delivery", request.OrderId);

            return CommandResult.Success();
        }
    }
}
