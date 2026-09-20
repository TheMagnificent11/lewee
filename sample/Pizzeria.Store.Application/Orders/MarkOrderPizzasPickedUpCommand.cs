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

public sealed record MarkOrderPizzasPickedUpCommand(Guid OrderId) : ICommand, ITenantRoleRequest
{
    public Guid TenantId => PizzaStore.Tenant.Id;

    public IReadOnlyCollection<string> Roles { get; } =
        [PizzaStore.Roles.StoreStaffCode, PizzaStore.Roles.StoreManagerCode];

    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "Used via mediation")]
    internal sealed class Validator : AbstractValidator<MarkOrderPizzasPickedUpCommand>
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
    internal sealed class Handler : IRequestHandler<MarkOrderPizzasPickedUpCommand, CommandResult>
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
            MarkOrderPizzasPickedUpCommand request,
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

            var result = order.PickedUp(correlationId);
            if (!result.IsSuccess)
            {
                return (CommandResult)result;
            }

            await this.orderRepository.SaveChangesAsync(cancellationToken);

            this.logger.LogInformation("Order {OrderId} marked as picked up", request.OrderId);

            return CommandResult.Success();
        }
    }
}
