using System.Diagnostics.CodeAnalysis;
using Lewee.Common;
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
        ArgumentNullException.ThrowIfNull(pizza);

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
            return CommandResult.Fail(ResultStatus.BadRequest, "Delivery address is required.");
        }

        this.DeliveryAddress = deliveryAddress;
        this.SubmittedDateTime = DateTime.UtcNow;

        this.DomainEvents.Raise(new DeliveryOrderSubmittedEvent(
            this.Id,
            this.UserId,
            this.SubmittedDateTime.Value,
            this.DeliveryAddress,
            correlationId));

        return CommandResult.Success();
    }

    public Result PizzasPrepared(Guid correlationId)
    {
        if (!this.IsSubmitted)
        {
            return CommandResult.Fail(ResultStatus.BadRequest, "Cannot prepare an order that is not submitted.");
        }

        if (this.IsPrepared)
        {
            return CommandResult.Success();
        }

        this.PreparedDateTime = DateTime.UtcNow;

        this.DomainEvents.Raise(new OrderPreparedEvent(
            this.Id,
            this.UserId,
            this.PreparedDateTime.Value,
            correlationId));

        return CommandResult.Success();
    }

    public Result PickedUp()
    {
        if (!this.IsPrepared)
        {
            return CommandResult.Fail(ResultStatus.BadRequest, "Cannot pick up an order that is not prepared.");
        }

        if (this.IsCompleted)
        {
            return CommandResult.Success();
        }

        this.CompletedDateTime = DateTime.UtcNow;

        return CommandResult.Success();
    }

    public Result PizzasDelivered()
    {
        if (!this.IsPrepared)
        {
            return CommandResult.Fail(ResultStatus.BadRequest, "Cannot deliver an order that is not prepared.");
        }

        if (this.IsCompleted)
        {
            return CommandResult.Success();
        }

        this.CompletedDateTime = DateTime.UtcNow;

        return CommandResult.Success();
    }

    public static class FieldLengths
    {
        public const int UserId = 100;
        public const int DeliveryAddress = 200;
    }
}
