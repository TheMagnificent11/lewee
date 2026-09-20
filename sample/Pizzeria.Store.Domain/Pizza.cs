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

    public static Pizza Create(string name, string description, decimal price)
    {
        ValidateName(name);
        ValidatePrice(price);

        return new Pizza(Guid.NewGuid(), name, description ?? string.Empty, price);
    }

    public void UpdateDetails(string name, string description, decimal price)
    {
        ValidateName(name);
        ValidatePrice(price);

        this.Name = name;
        this.Description = description ?? string.Empty;
        this.Price = price;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Pizza name is required.", nameof(name));
        }

        if (name.Length > FieldLengths.Name)
        {
            throw new ArgumentException(
                $"Pizza name cannot exceed {FieldLengths.Name} characters.",
                nameof(name));
        }
    }

    private static void ValidatePrice(decimal price)
    {
        if (price <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "Pizza price must be greater than zero.");
        }
    }

    public static class FieldLengths
    {
        public const int Name = 25;
        public const int Description = 500;
    }
}
