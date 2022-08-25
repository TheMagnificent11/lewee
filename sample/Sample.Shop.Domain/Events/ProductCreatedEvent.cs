using Saji.Domain;
using Sample.Shop.Domain.Entities;

namespace Sample.Shop.Domain.Events;

public class ProductCreatedEvent : IDomainEvent
{
    public ProductCreatedEvent(Guid correlationId, Product product)
    {
        this.CorrelationId = correlationId;
        this.ProductId = product.Id;
        this.Name = product.Name;
        this.SalePrice = product.SalePrice;
    }

    public Guid CorrelationId { get; }
    public Guid ProductId { get; }
    public string Name { get; }
    public decimal SalePrice { get; }
}
