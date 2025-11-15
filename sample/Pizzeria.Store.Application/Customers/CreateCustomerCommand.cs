using FluentValidation;
using Lewee.Application.Mediation.Requests;
using Lewee.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application.Customers;

public record CreateCustomerCommand(string ExternalId, Guid CorrelationId) : ICommand
{
    internal class Validator : AbstractValidator<CreateCustomerCommand>
    {
        public Validator()
        {
            this.RuleFor(x => x.ExternalId)
                .NotEmpty()
                .MaximumLength(Customer.FieldLengths.ExternalId);
        }
    }

    internal sealed class Handler : IRequestHandler<CreateCustomerCommand, CommandResult>
    {
        private readonly IRepository<Customer> repository;
        private readonly ILogger<Handler> logger;

        public Handler(IRepository<Customer> repository, ILogger<Handler> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        public async Task<CommandResult> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = Customer.Create(request.ExternalId, request.CorrelationId);

            await this.repository.AddAsync(customer, cancellationToken);
            await this.repository.SaveChangesAsync(cancellationToken);

            this.logger.LogCustomerCreated(
                customer.Id,
                request.ExternalId);

            return CommandResult.Success();
        }
    }
}
