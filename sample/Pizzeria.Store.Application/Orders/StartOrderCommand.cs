using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lewee.Application.Mediation.Requests;
using Lewee.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application.Orders;

public record StartOrderCommand(string UserId, Guid CorrelationId) : ICommand
{
    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "Used via mediation")]
    internal sealed class Validator : AbstractValidator<StartOrderCommand>
    {
        public Validator()
        {
            this.RuleFor(x => x.UserId)
                .NotEmpty()
                .MaximumLength(Order.FieldLengths.UserId);
        }
    }

    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "Used via mediation")]
    internal sealed class Handler : IRequestHandler<StartOrderCommand, CommandResult>
    {
        private readonly IRepository<Order> repository;
        private readonly ILogger<Handler> logger;

        public Handler(IRepository<Order> repository, ILogger<Handler> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        public async Task<CommandResult> Handle(StartOrderCommand request, CancellationToken cancellationToken)
        {
            var order = Order.StartNewOrder(request.UserId, request.CorrelationId);

            await this.repository.AddAsync(order, cancellationToken);
            await this.repository.SaveChangesAsync(cancellationToken);

            this.logger.LogOrderStarted(
                order.Id,
                request.UserId);

            return CommandResult.Success();
        }
    }
}
