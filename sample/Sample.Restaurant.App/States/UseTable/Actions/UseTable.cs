namespace Sample.Restaurant.App.States.UseTable.Actions;
public record UseTable
{
    public UseTable(Guid correlationId, int tableNumber)
    {
        this.CorrelationId = correlationId;
        this.TableNumber = tableNumber;
    }

    public Guid CorrelationId { get; }
    public int TableNumber { get; }
}
