using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using Lewee.Domain;

namespace Pizzeria.Store.Domain;

public class Order : AggregateRoot
{
    private readonly List<OrderPizza> pizzas;

    internal Order(string userId, Guid correlationId)
        : base()
    {
        this.pizzas = [];
        this.UserId = userId;
        this.StartedDateTime = DateTime.UtcNow;

        this.DomainEvents.Raise(new OrderStartedEvent(
            this.Id,
            this.UserId,
            this.StartedDateTime,
            correlationId));
    }

    [ExcludeFromCodeCoverage(Justification = "Only used by EF")]
    private Order()
        : base()
    {
        this.pizzas = [];
    }

    public string UserId { get; protected set; }

    public IReadOnlyCollection<OrderPizza> Pizzas => this.pizzas;

    public string? DeliveryAddress { get; protected set; }

    public bool IsDeliveryOrder => !string.IsNullOrWhiteSpace(this.DeliveryAddress);

    public DateTime StartedDateTime { get; protected set; }

    public DateTime? SubmittedDateTime { get; protected set; }

    public bool IsSubmitted => this.SubmittedDateTime is not null;

    public DateTime? PreparedDateTime { get; protected set; }

    public bool IsPrepared => this.PreparedDateTime is not null;

    public DateTime? CompletedDateTime { get; protected set; }

    public bool IsCompleted => this.CompletedDateTime is not null;

    public static Order StartNewOrder(string userId, Guid correlationId)
    {
        return new Order(userId, correlationId);
    }

    public void AddPizza(Pizza pizza)
    {
        var existingOrderPizza = this.pizzas.FirstOrDefault(x => x.PizzaId == pizza.Id);
        if (existingOrderPizza is null)
        {
            this.pizzas.Add(OrderPizza.CreateForOrder(this, pizza));
            return;
        }

        existingOrderPizza.IncreaseQuantity();
    }

    public void SubmitPickupOrder(Guid correlationId)
    {
        this.DeliveryAddress = null;
        this.SubmittedDateTime = DateTime.UtcNow;

        this.DomainEvents.Raise(new PickupOrderSubmittedEvent(
            this.Id,
            this.UserId,
            this.SubmittedDateTime.Value,
            correlationId));
    }

    public Result SubmitDeliveryOrder(string deliveryAddress, Guid correlationId)
    {
        if (string.IsNullOrWhiteSpace(deliveryAddress))
        {
            return Result.Failure("Delivery address is required.");
        }

        this.DeliveryAddress = deliveryAddress;
        this.SubmittedDateTime = DateTime.UtcNow;

        this.DomainEvents.Raise(new DeliveryOrderSubmittedEvent(
            this.Id,
            this.UserId,
            this.SubmittedDateTime.Value,
            this.DeliveryAddress,
            correlationId));

        return Result.Success();
    }

    public Result PizzasPrepared(Guid correlationId)
    {
        if (!this.IsSubmitted)
        {
            return Result.Failure("Cannot prepare an order that is not submitted.");
        }

        if (this.IsPrepared)
        {
            return Result.Success("Pizzas already prepared.");
        }

        this.PreparedDateTime = DateTime.UtcNow;

        this.DomainEvents.Raise(new OrderPreparedEvent(
            this.Id,
            this.UserId,
            this.PreparedDateTime.Value,
            correlationId));

        return Result.Success();
    }

    public Result PickedUp()
    {
        if (!this.IsPrepared)
        {
            return Result.Failure("Cannot pick up an order that is not prepared.");
        }

        if (this.IsCompleted)
        {
            return Result.Success("Order already picked up.");
        }

        this.CompletedDateTime = DateTime.UtcNow;

        return Result.Success();
    }

    public Result PizzasDelivered()
    {
        if (!this.IsPrepared)
        {
            return Result.Failure("Cannot deliver an order that is not prepared.");
        }

        if (this.IsCompleted)
        {
            return Result.Success("Pizza already delivered.");
        }

        this.CompletedDateTime = DateTime.UtcNow;

        return Result.Success();
    }

    public static class FieldLengths
    {
        public const int UserId = 100;
        public const int DeliveryAddress = 200;
    }
}
