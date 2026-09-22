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
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application.Orders;

public sealed record MarkOrderPizzasDeliveredCommand(Guid OrderId) : ICommand, ITenantRoleRequest
{
    public Guid TenantId => PizzaStore.Tenant.Id;

    public IReadOnlyCollection<string> Roles { get; } =
        [PizzaStore.Roles.StoreStaffCode, PizzaStore.Roles.StoreManagerCode];

    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "Used via mediation")]
    internal sealed class Validator : AbstractValidator<MarkOrderPizzasDeliveredCommand>
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
    internal sealed class Handler : IRequestHandler<MarkOrderPizzasDeliveredCommand, CommandResult>
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
            MarkOrderPizzasDeliveredCommand request,
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

            var result = order.Delivered(correlationId);
            if (!result.IsSuccess)
            {
                return (CommandResult)result;
            }

            await this.orderRepository.SaveChangesAsync(cancellationToken);

            this.logger.LogInformation("Order {OrderId} marked as delivered", request.OrderId);

            return CommandResult.Success();
        }
    }
}
