using Sample.Restaurant.Application;

namespace Sample.Restaurant.App.States.Tables;

public record TablesState
{
    public string RequestType => "GetTables";
    public bool IsLoading { get; init; }
    public Guid CorrelationId { get; init; } = Guid.Empty;
    public TableDto[] Tables { get; init; } = Array.Empty<TableDto>();
    public string? ErrorMessage { get; init; }
}
