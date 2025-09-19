using FreeMediator;
using Lewee.Application.Mediation.Requests;
using Lewee.Domain;

using Microsoft.Extensions.Logging;
using Sample.Restaurant.Application.QuerySpecifications;
using Sample.Restaurant.Domain;

namespace Sample.Restaurant.Application;

public sealed class RemoveMenuItemCommand : ICommand, ITableRequest
{
    public RemoveMenuItemCommand(Guid correlationId, int tableNumber, Guid menuItemId)
    {
        this.CorrelationId = correlationId;
        this.TableNumber = tableNumber;
        this.MenuItemId = menuItemId;
    }

    public Guid CorrelationId { get; }
    public int TableNumber { get; }
    public Guid MenuItemId { get; }

    internal class RemoveMenuItemCommandHandler : IRequestHandler<RemoveMenuItemCommand, CommandResult>
    {
        private readonly IRepository<Table> tableRepository;
        private readonly IRepository<MenuItem> menuItemRepository;
        private readonly ILogger logger;

        public RemoveMenuItemCommandHandler(
            IRepository<Table> tableRepository,
            IRepository<MenuItem> menuItemRepository,
            ILogger<RemoveMenuItemCommandHandler> logger)
        {
            this.tableRepository = tableRepository;
            this.menuItemRepository = menuItemRepository;
            this.logger = logger;
        }

        public async Task<CommandResult> Handle(RemoveMenuItemCommand request, CancellationToken cancellationToken)
        {
            var table = await this.tableRepository.QueryOne(
                new TableOrderQuerySpecification(request.TableNumber),
                cancellationToken);

            if (table == null)
            {
                return CommandResult.Fail(ResultStatus.NotFound, "Table not found");
            }

            var menuItem = await this.menuItemRepository.RetrieveById(request.MenuItemId, cancellationToken);

            if (menuItem == null)
            {
                return CommandResult.Fail(ResultStatus.NotFound, "Menu item not found");
            }

            table.RemovedMenuItem(menuItem, request.CorrelationId);

            await this.tableRepository.SaveChanges(cancellationToken);

            this.logger.LogInformation("Menu Item {@MenuItem} removed from table order", menuItem);

            return CommandResult.Success();
        }
    }
}
