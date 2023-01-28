namespace Sample.Restaurant.App.States.Tables.Actions;

public record GetTables
{
    public GetTables(Guid correlationId)
    {
        this.CorrelationId = correlationId;
    }

    public Guid CorrelationId { get; }
}
