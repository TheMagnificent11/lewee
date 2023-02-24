using Lewee.Application.Mediation;
using Lewee.Domain;
using Lewee.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sample.Restaurant.Domain;
using Serilog;

namespace Sample.Restaurant.Application;

public class AddMenuItemCommand : ICommand, ITableRequest
{
    public AddMenuItemCommand(Guid correlationId, int tableNumber, Guid menuItemId)
    {
        this.CorrelationId = correlationId;
        this.TableNumber = tableNumber;
        this.MenuItemId = menuItemId;
    }

    public Guid CorrelationId { get; }
    public int TableNumber { get; }
    public Guid MenuItemId { get; }

    internal class AddMenuItemCommandHandler : IRequestHandler<AddMenuItemCommand, CommandResult>
    {
        private readonly IRepository<Table> tableRepository;
        private readonly IRepository<MenuItem> menuItemRepository;
        private readonly ILogger logger;

        public AddMenuItemCommandHandler(IRepository<Table> tableRepository, IRepository<MenuItem> menuItemRepository, ILogger logger)
        {
            this.tableRepository = tableRepository;
            this.menuItemRepository = menuItemRepository;
            this.logger = logger.ForContext<AddMenuItemCommandHandler>();
        }

        public async Task<CommandResult> Handle(AddMenuItemCommand request, CancellationToken cancellationToken)
        {
            var table = await this.tableRepository
                .All()
                .Where(x => x.TableNumber == request.TableNumber)
                .Include(x => x.Orders)
                .ThenInclude(x => x.Items)
                .FirstOrDefaultAsync(cancellationToken);

            if (table == null)
            {
                return CommandResult.Fail(ResultStatus.NotFound, "Table not found");
            }

            var menuItem = await this.menuItemRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == request.MenuItemId, cancellationToken);

            if (menuItem == null)
            {
                return CommandResult.Fail(ResultStatus.NotFound, "Menu item not found");
            }

            table.OrderMenuItem(menuItem, request.CorrelationId);

            await this.tableRepository.SaveChanges(cancellationToken);

            this.logger.Information("Menu Item {@MenuItem} added to table order", menuItem);

            return CommandResult.Success();
        }
    }
}
