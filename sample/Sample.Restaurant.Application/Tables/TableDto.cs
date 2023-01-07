using Sample.Restaurant.Contracts;

namespace Sample.Restaurant.Application.Tables;

public class TableDto
{
    public Guid Id { get; set; }
    public int TableNumber { get; set; }
    public TableStatus StatusId { get; set; }
}
