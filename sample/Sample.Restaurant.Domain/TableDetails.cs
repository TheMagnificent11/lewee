using System.Runtime.CompilerServices;
using Lewee.Domain;

namespace Sample.Restaurant.Domain;

public record TableDetails : IQueryProjection
{
    public Guid CorrelationId { get; set; }
    public Guid Id { get; set; }
    public int TableNumber { get; set; }
    public bool IsInUse { get; set; }
    public List<OrderItemDetails> Items { get; set; } = new();

    public static TableDetails FromTableInUseDomainEvent(TableInUseDomainEvent domainEvent, IEnumerable<MenuItem> menuItems)
    {
        var tableDetails = new TableDetails
        {
            CorrelationId = domainEvent.CorrelationId,
            Id = domainEvent.TableId,
            TableNumber = domainEvent.TableNumber,
            IsInUse = true
        };

        foreach (var item in menuItems)
        {
            tableDetails.Items.Add(OrderItemDetails.FromMenuItem(item));
        }

        return tableDetails;
    }
}
