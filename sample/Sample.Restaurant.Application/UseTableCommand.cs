using FreeMediator;
using Lewee.Application.Mediation.Requests;
using Lewee.Domain;

using Microsoft.Extensions.Logging;
using Sample.Restaurant.Application.QuerySpecifications;
using Sample.Restaurant.Domain;

namespace Sample.Restaurant.Application;

public sealed class UseTableCommand : ICommand, ITableRequest
{
    public UseTableCommand(Guid correlationId, int tableNumber)
    {
        this.CorrelationId = correlationId;
        this.TableNumber = tableNumber;
    }

    public Guid CorrelationId { get; }
    public int TableNumber { get; }

    internal class UseTableCommandHandler : IRequestHandler<UseTableCommand, CommandResult>
    {
        private readonly IRepository<Table> repository;
        private readonly ILogger logger;

        public UseTableCommandHandler(
            IRepository<Table> repository,
            ILogger<UseTableCommand> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        public async Task<CommandResult> Handle(UseTableCommand request, CancellationToken cancellationToken)
        {
            var table = await this.repository.QueryOne(
                new TableOrderQuerySpecification(request.TableNumber),
                cancellationToken);

            if (table == null)
            {
                return CommandResult.Fail(ResultStatus.NotFound, "Table not found");
            }

            table.Use(request.CorrelationId);

            await this.repository.SaveChanges(cancellationToken);

            this.logger.LogInformation("Table is in use and is no longer available");

            return CommandResult.Success();
        }
    }
}
