using FreeMediator;
using Lewee.Application.Mediation.Notifications;
using Lewee.Domain;
using Lewee.Shared;
using Microsoft.Extensions.Logging;
using Sample.Restaurant.Contracts.ClientMessages;
using Sample.Restaurant.Domain;

namespace Sample.Restaurant.Application;

internal class TableDomainEventHandler :
    INotificationHandler<TableInUseDomainEvent>,
    INotificationHandler<OrderItemAddedDomainEvent>,
    INotificationHandler<OrderItemRemovedDomainEvent>
{
    private readonly IRepository<MenuItem> menuItemRepository;
    private readonly IQueryProjectionService queryProjectionService;
    private readonly IMediator mediator;
    private readonly ILogger logger;

    public TableDomainEventHandler(
        IRepository<MenuItem> menuItemRepository,
        IQueryProjectionService queryProjectionService,
        IMediator mediator,
        ILogger<TableDomainEventHandler> logger)
    {
        this.menuItemRepository = menuItemRepository;
        this.queryProjectionService = queryProjectionService;
        this.mediator = mediator;
        this.logger = logger;
    }

    public async Task Handle(TableInUseDomainEvent notification, CancellationToken cancellationToken)
    {
        using (this.logger.BeginScope(new Dictionary<string, object>
        {
            {  LoggingConsts.CorrelationId, notification.CorrelationId },
            {  nameof(notification.TableNumber), notification.TableNumber }
        }))
        {
            var menuItems = await this.menuItemRepository.All(cancellationToken);

            var sortedMenuItems = menuItems
                .OrderBy(x => x.ItemTypeId)
                .ThenBy(x => x.Name)
                .ToArray();

            var projection = TableDetails.FromTableInUseDomainEvent(notification, menuItems);

            await this.queryProjectionService.AddOrUpdate(
                projection,
                notification.TableNumber.ToString(),
                cancellationToken);

            this.logger.LogDebug("TableDetails query projection created");

            var message = new TableUsedMessage { TableNumber = notification.TableNumber };
            await this.PublishClientEvent(message, notification.CorrelationId, cancellationToken);
        }
    }

    public async Task Handle(OrderItemAddedDomainEvent notification, CancellationToken cancellationToken)
    {
        using (this.logger.BeginScope(new Dictionary<string, object>
        {
            {  LoggingConsts.CorrelationId, notification.CorrelationId },
            {  nameof(notification.TableNumber), notification.TableNumber }
        }))
        {
            var projection = await this.queryProjectionService.RetrieveByKey<TableDetails>(
                notification.TableNumber.ToString(),
                cancellationToken);

            if (projection == null)
            {
                this.logger.LogError("TableDetails query projection not found");
                return;
            }

            var updatedProjection = projection.AddOrderItem(notification);

            await this.queryProjectionService.AddOrUpdate(
                updatedProjection,
                notification.TableNumber.ToString(),
                cancellationToken);

            this.logger.LogDebug("TableDetails query projection updated for OrderItemAddedDomainEvent");

            var message = new ItemOrderedMessage { TableNumber = notification.TableNumber };
            await this.PublishClientEvent(message, notification.CorrelationId, cancellationToken);
        }
    }

    public async Task Handle(OrderItemRemovedDomainEvent notification, CancellationToken cancellationToken)
    {
        using (this.logger.BeginScope(new Dictionary<string, object>
        {
            {  LoggingConsts.CorrelationId, notification.CorrelationId },
            {  nameof(notification.TableNumber), notification.TableNumber }
        }))
        {
            var projection = await this.queryProjectionService.RetrieveByKey<TableDetails>(
                notification.TableNumber.ToString(),
                cancellationToken);

            if (projection == null)
            {
                this.logger.LogError("TableDetails query projection not found");
                return;
            }

            var updatedProjection = projection.RemoveOrderItem(notification);

            await this.queryProjectionService.AddOrUpdate(
                updatedProjection,
                notification.TableNumber.ToString(),
                cancellationToken);

            this.logger.LogDebug("TableDetails query projection updated for OrderItemRemovedDomainEvent");

            var message = new ItemRemovedMessage { TableNumber = notification.TableNumber };
            await this.PublishClientEvent(message, notification.CorrelationId, cancellationToken);
        }
    }

    private async Task PublishClientEvent(object message, Guid correlationId, CancellationToken cancellationToken)
    {
        var clientEvent = new ClientEvent(correlationId, null, message);
        await this.mediator.Publish(clientEvent, cancellationToken);

        this.logger.LogDebug("{ClientEventType} client event published", message.GetType().Name);
    }
}
