namespace Sample.Restaurant.Application;

public class TableDto
{
    public Guid Id { get; set; }
    public int TableNumber { get; set; }
    public bool IsInUse { get; set; }

    public string Status => this.IsInUse ? "In Use" : "Available";
}
