using Lewee.Contracts;

namespace Sample.Restaurant.Contracts.ClientMessages;

public class ItemOrderedMessage : IClientMessageContract
{
    public int TableNumber { get; init; }
}
