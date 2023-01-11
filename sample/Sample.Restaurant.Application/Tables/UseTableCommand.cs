using Lewee.Application.Mediation;
using Lewee.Application.Mediation.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sample.Restaurant.Domain;
using Sample.Restaurant.Domain.Queries;
using Serilog;
using Serilog.Context;

namespace Sample.Restaurant.Application.Tables;

public sealed class UseTableCommand : ICommand
{
    public UseTableCommand(Guid correlationId, int tableNumber)
    {
        this.CorrelationId = correlationId;
        this.TableNumber = tableNumber;
    }

    public Guid? TenantId { get; }
    public Guid CorrelationId { get; }
    public int TableNumber { get; }

    internal class UseTableCommandHandler : IRequestHandler<UseTableCommand, CommandResult>
    {
        private readonly IRestaurantDbContext dbContext;
        private readonly ILogger logger;

        public UseTableCommandHandler(IRestaurantDbContext dbContext, ILogger logger)
        {
            this.dbContext = dbContext;
            this.logger = logger.ForContext<UseTableCommandHandler>();
        }

        public async Task<CommandResult> Handle(UseTableCommand request, CancellationToken cancellationToken)
        {
            using (LogContext.PushProperty(nameof(request.TableNumber), request.TableNumber))
            {
                var table = await this.dbContext
                    .AggregateRoot<Table>()
                    .FindByNumber(request.TableNumber)
                    .FirstOrDefaultAsync(cancellationToken);

                if (table == null)
                {
                    return CommandResult.Fail(ResultStatus.NotFound, "Could not find table");
                }

                table.Use(request.CorrelationId);

                await this.dbContext.SaveChangesAsync(cancellationToken);

                this.logger.Information("Table is in use and is no longer available");

                return CommandResult.Success();
            }
        }
    }
}
