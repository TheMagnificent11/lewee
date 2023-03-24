namespace Sample.Restaurant.Application;

public class TableDto
{
    public Guid Id { get; set; }
    public int TableNumber { get; set; }
    public bool IsInUse { get; set; }

    public string Status => this.IsInUse ? Statuses.InUse : Statuses.Available;

    public static class Statuses
    {
        public const string Available = "Avaiable";
        public const string InUse = "In Use";
    }
}
