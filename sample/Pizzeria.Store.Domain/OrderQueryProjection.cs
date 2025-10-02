using Lewee.Domain;

namespace Pizzeria.Store.Domain;

public class OrderQueryProjection : Entity, IQueryProjection
{
    public Guid CorrelationId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime StartedDateTime { get; set; }
    public DateTime? SubmittedDateTime { get; set; }
    public DateTime? PreparedDateTime { get; set; }
    public DateTime? CompletedDateTime { get; set; }
    public string? DeliveryAddress { get; set; }
    public string PizzasJson { get; set; } = "[]";
    public decimal TotalCost { get; set; }
}
