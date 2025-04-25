using System.Diagnostics.CodeAnalysis;
using Lewee.Domain;

namespace Pizzeria.Store.Domain;

public class Pizza : AggregateRoot
{
    internal Pizza(Guid id, string name, string description, decimal price)
        : base(id)
    {
        this.Name = name;
        this.Description = description;
        this.Price = price;
    }

    [ExcludeFromCodeCoverage(Justification = "Only used by EF")]
    private Pizza()
        : base()
    {
    }

    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public decimal Price { get; protected set; }
    public IReadOnlyCollection<OrderPizza> OrderPizzas { get; protected set; }

    public static class FieldLengths
    {
        public const int Name = 25;
        public const int Description = 500;
    }
}
