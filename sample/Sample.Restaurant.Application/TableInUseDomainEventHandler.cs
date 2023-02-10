using Lewee.Domain;
using Lewee.Shared;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sample.Restaurant.Domain;
using Serilog;
using Serilog.Context;

namespace Sample.Restaurant.Application;

internal class TableInUseDomainEventHandler : INotificationHandler<TableInUseDomainEvent>
{
    private readonly IRepository<Table> tableRepository;
    private readonly IRepository<MenuItem> menuItemRepository;
    private readonly IReadModelService readModelService;
    private readonly IMapper mapper;
    private readonly ILogger logger;

    public TableInUseDomainEventHandler(
        IRepository<Table> tableRepository,
        IRepository<MenuItem> menuItemRepository,
        IReadModelService readModelService,
        IMapper mapper,
        ILogger logger)
    {
        this.tableRepository = tableRepository;
        this.menuItemRepository = menuItemRepository;
        this.readModelService = readModelService;
        this.mapper = mapper;
        this.logger = logger.ForContext<TableInUseDomainEventHandler>();
    }

    public async Task Handle(TableInUseDomainEvent notification, CancellationToken cancellationToken)
    {
        using (LogContext.PushProperty(LoggingConsts.CorrelationId, notification.CorrelationId))
        using (LogContext.PushProperty(nameof(notification.TableNumber), notification.TableNumber))
        {
            var table = await this.tableRepository
                .All()
                .Include(x => x.Orders)
                .ThenInclude(x => x.Items)
                .FirstOrDefaultAsync(x => x.TableNumber == notification.TableNumber, cancellationToken);

            if (table == null)
            {
                this.logger.Warning("Table not found");
                return;
            }

            var tableReadModel = this.mapper.Map<TableDetailsReadModel>(table);
            tableReadModel.CorrelationId = notification.CorrelationId;

            var menuItems = await this.menuItemRepository
                .All()
                .OrderBy(x => x.ItemTypeId)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);

            var menuItemReadModels = this.mapper.Map<MenuItemReadModel[]>(menuItems);

            if (table.CurrentOrder != null)
            {
                foreach (var menuItem in menuItemReadModels)
                {
                    var orderItemReadModel = tableReadModel.Items.FirstOrDefault(x => x.MenuItem.Id == menuItem.Id);
                    if (orderItemReadModel == null)
                    {
                        tableReadModel.Items.Add(new OrderItemReadModel { MenuItem = menuItem });
                        continue;
                    }

                    // Shouldn't get here as a new order should have no order items
                    orderItemReadModel.MenuItem = menuItem;
                }
            }

            await this.readModelService.AddOrUpdate(tableReadModel, table.TableNumber.ToString(), cancellationToken);

            this.logger.Debug("Table read model created");
        }
    }
}
