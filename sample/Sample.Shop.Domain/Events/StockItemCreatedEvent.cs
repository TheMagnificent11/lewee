﻿using Saji.Domain;
using Sample.Shop.Domain.Entities;

namespace Sample.Shop.Domain.Events;

public class StockItemCreatedEvent : IDomainEvent
{
    public StockItemCreatedEvent(Guid correlationId, StockItem item)
    {
        this.CorrelationId = correlationId;
        this.CatalogueItemId = item.Id;
        this.Name = item.Name;
        this.Description = item.Description;
        this.SalePrice = item.SalePrice;
    }

    public Guid CorrelationId { get; }
    public Guid CatalogueItemId { get; }
    public string Name { get; }
    public string Description { get; }
    public decimal SalePrice { get; }
}
