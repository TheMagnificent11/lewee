namespace Pizzeria.Store.Contracts;

public class OrderStartedEventDto
{
    public Guid OrderId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime StartedDateTime { get; set; }
}
