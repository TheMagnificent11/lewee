namespace Sample.Restaurant.Contracts.ClientMessages;

public class TableUsedMessage
{
    public Guid CorrelationId { get; set; }
    public int TableNumber { get; set; }
}
