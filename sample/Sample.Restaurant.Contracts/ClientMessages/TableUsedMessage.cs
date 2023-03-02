using Lewee.Contracts;

namespace Sample.Restaurant.Contracts.ClientMessages;

public class TableUsedMessage : IClientMessageContract
{
    public Guid CorrelationId { get; set; }
    public int TableNumber { get; set; }
}
