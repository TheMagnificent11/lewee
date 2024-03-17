using Lewee.Domain;
using Lewee.Shared;
using MediatR;
using Sample.Restaurant.Domain;
using Serilog;
using Serilog.Context;

namespace Sample.Restaurant.Application;

internal class TableDomainEventHandler :
    INotificationHandler<TableInUseDomainEvent>,
    INotificationHandler<OrderItemAddedDomainEvent>,
    INotificationHandler<OrderItemRemovedDomainEvent>
{
    private readonly IRepository<MenuItem> menuItemRepository;
    private readonly IQueryProjectionService queryProjectionService;
    private readonly ILogger logger;

    public TableDomainEventHandler(
        IRepository<MenuItem> menuItemRepository,
        IQueryProjectionService queryProjectionService,
        ILogger logger)
    {
        this.menuItemRepository = menuItemRepository;
        this.queryProjectionService = queryProjectionService;
        this.logger = logger.ForContext<TableDomainEventHandler>();
    }

    public async Task Handle(TableInUseDomainEvent notification, CancellationToken cancellationToken)
    {
        using (LogContext.PushProperty(LoggingConsts.CorrelationId, notification.CorrelationId))
        using (LogContext.PushProperty(nameof(notification.TableNumber), notification.TableNumber))
        {
            var menuItems = await this.menuItemRepository.All(cancellationToken);

            var sortedMenuItems = menuItems
                .OrderBy(x => x.ItemTypeId)
                .ThenBy(x => x.Name)
                .ToArray();

            var projection = TableDetails.FromTableInUseDomainEvent(notification, sortedMenuItems);

            await this.queryProjectionService.AddOrUpdate(
                projection,
                notification.TableNumber.ToString(),
                cancellationToken);

            this.logger.Debug("TableDetails query projection created");
        }
    }

    public async Task Handle(OrderItemAddedDomainEvent notification, CancellationToken cancellationToken)
    {
        using (LogContext.PushProperty(LoggingConsts.CorrelationId, notification.CorrelationId))
        using (LogContext.PushProperty(nameof(notification.TableNumber), notification.TableNumber))
        {
            var projection = await this.queryProjectionService.RetrieveByKey<TableDetails>(
                notification.TableNumber.ToString(),
                cancellationToken);

            if (projection == null)
            {
                this.logger.Error("TableDetails query projection not found");
                return;
            }

            var updatedProjection = projection.AddOrderItem(notification);

            await this.queryProjectionService.AddOrUpdate(
                updatedProjection,
                notification.TableNumber.ToString(),
                cancellationToken);

            this.logger.Debug("TableDetails query projection updated for OrderItemAddedDomainEvent");
        }
    }

    public async Task Handle(OrderItemRemovedDomainEvent notification, CancellationToken cancellationToken)
    {
        using (LogContext.PushProperty(LoggingConsts.CorrelationId, notification.CorrelationId))
        using (LogContext.PushProperty(nameof(notification.TableNumber), notification.TableNumber))
        {
            var projection = await this.queryProjectionService.RetrieveByKey<TableDetails>(
                notification.TableNumber.ToString(),
                cancellationToken);

            if (projection == null)
            {
                this.logger.Error("TableDetails query projection not found");
                return;
            }

            var updatedProjection = projection.RemoveOrderItem(notification);

            await this.queryProjectionService.AddOrUpdate(
                updatedProjection,
                notification.TableNumber.ToString(),
                cancellationToken);

            this.logger.Debug("TableDetails query projection updated for OrderItemRemovedDomainEvent");
        }
    }
}
