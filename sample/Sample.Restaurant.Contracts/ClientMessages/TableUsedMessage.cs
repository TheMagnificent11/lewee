using Lewee.Contracts;

namespace Sample.Restaurant.Contracts.ClientMessages;

public class TableUsedMessage : IClientMessageContract
{
    public int TableNumber { get; init; }
}
