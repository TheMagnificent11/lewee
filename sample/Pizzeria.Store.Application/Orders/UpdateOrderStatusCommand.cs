using System.Diagnostics.CodeAnalysis;
using Correlate;
using FluentValidation;
using Lewee.Application.Mediation.Behaviors;
using Lewee.Application.Mediation.Requests;
using Lewee.Auth.Application;
using Lewee.Common;
using Lewee.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using Pizzeria.Common;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application.Orders;

public sealed record UpdateOrderStatusCommand(Guid OrderId, OrderFulfillmentAction Action)
    : ICommand, ITenantRoleRequest
{
    public Guid TenantId => PizzaStore.Tenant.Id;

    public IReadOnlyCollection<string> Roles { get; } =
        [PizzaStore.Roles.StoreStaffCode, PizzaStore.Roles.StoreManagerCode];

    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "Used via mediation")]
    internal sealed class Validator : AbstractValidator<UpdateOrderStatusCommand>
    {
        public Validator()
        {
            this.RuleFor(x => x.OrderId)
                .NotEmpty();

            this.RuleFor(x => x.Action)
                .IsInEnum();
        }
    }

    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "Used via mediation")]
    internal sealed class Handler : IRequestHandler<UpdateOrderStatusCommand, CommandResult>
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
            UpdateOrderStatusCommand request,
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
            var result = request.Action switch
            {
                OrderFulfillmentAction.StartMaking => order.StartMaking(correlationId),
                OrderFulfillmentAction.Prepared => order.Prepared(correlationId),
                OrderFulfillmentAction.PickedUp => order.PickedUp(correlationId),
                OrderFulfillmentAction.OutForDelivery => order.OutForDelivery(correlationId),
                OrderFulfillmentAction.Delivered => order.Delivered(correlationId),
                _ => CommandResult.Fail(ResultStatus.BadRequest, "Unknown order fulfillment action."),
            };

            if (!result.IsSuccess)
            {
                return (CommandResult)result;
            }

            await this.orderRepository.SaveChangesAsync(cancellationToken);

            this.logger.LogInformation(
                "Order {OrderId} fulfillment action {Action} completed",
                request.OrderId,
                request.Action);

            return CommandResult.Success();
        }
    }
}
