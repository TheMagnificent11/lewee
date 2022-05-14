using MediatR;
using Saji.Application.Mediation;
using Saji.Application.Mediation.Responses;

namespace Sample.Customers.Application
{
    public class CreateCustomerCommand : ICommand
    {
        public CreateCustomerCommand(
            Guid correlationId,
            Guid tenantId,
            string emailAddress,
            string givenName,
            string surname)
        {
            this.CorrelationId = correlationId;
            this.TenantId = tenantId;
            this.EmailAddress = emailAddress;
            this.GivenName = givenName;
            this.Surname = surname;
        }

        public Guid CorrelationId { get; }
        public Guid? TenantId { get; }
        public string EmailAddress { get; }
        public string GivenName { get; }
        public string Surname { get; }

        internal class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CommandResult>
        {
            public Task<CommandResult> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
