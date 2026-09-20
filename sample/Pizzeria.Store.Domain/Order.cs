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
        this.Status = OrderStatus.InProgress;

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

    public OrderStatus Status { get; protected set; }

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

    public Result AddPizza(Pizza pizza)
    {
        ArgumentNullException.ThrowIfNull(pizza);

        if (this.IsSubmitted)
        {
            return CommandResult.Fail(
                ResultStatus.BadRequest,
                "Cannot add pizzas to an order that has already been submitted.");
        }

        var existingOrderPizza = this.pizzas.FirstOrDefault(x => x.PizzaId == pizza.Id);
        if (existingOrderPizza is null)
        {
            this.pizzas.Add(OrderPizza.CreateForOrder(this, pizza));
            return CommandResult.Success();
        }

        existingOrderPizza.IncreaseQuantity();

        return CommandResult.Success();
    }

    public Result RemovePizza(Pizza pizza)
    {
        ArgumentNullException.ThrowIfNull(pizza);

        if (this.IsSubmitted)
        {
            return CommandResult.Fail(
                ResultStatus.BadRequest,
                "Cannot remove pizzas from an order that has already been submitted.");
        }

        var existingOrderPizza = this.pizzas.FirstOrDefault(x => x.PizzaId == pizza.Id);
        if (existingOrderPizza is null)
        {
            return CommandResult.Fail(ResultStatus.NotFound, $"Pizza {pizza.Id} is not part of this order.");
        }

        existingOrderPizza.DecreaseQuantity();

        if (existingOrderPizza.Quantity <= 0)
        {
            this.pizzas.Remove(existingOrderPizza);
        }

        return CommandResult.Success();
    }

    public Result SubmitPickupOrder(Guid correlationId)
    {
        var canSubmit = this.EnsureCanSubmit();
        if (!canSubmit.IsSuccess)
        {
            return canSubmit;
        }

        this.DeliveryAddress = null;
        this.SubmittedDateTime = DateTime.UtcNow;
        this.Status = OrderStatus.Received;

        this.DomainEvents.Raise(new PickupOrderSubmittedEvent(
            this.Id,
            this.UserId,
            this.SubmittedDateTime.Value,
            correlationId));

        return CommandResult.Success();
    }

    public Result SubmitDeliveryOrder(string deliveryAddress, Guid correlationId)
    {
        if (string.IsNullOrWhiteSpace(deliveryAddress))
        {
            return CommandResult.Fail(ResultStatus.BadRequest, "Delivery address is required.");
        }

        var canSubmit = this.EnsureCanSubmit();
        if (!canSubmit.IsSuccess)
        {
            return canSubmit;
        }

        this.DeliveryAddress = deliveryAddress;
        this.SubmittedDateTime = DateTime.UtcNow;
        this.Status = OrderStatus.Received;

        this.DomainEvents.Raise(new DeliveryOrderSubmittedEvent(
            this.Id,
            this.UserId,
            this.SubmittedDateTime.Value,
            this.DeliveryAddress,
            correlationId));

        return CommandResult.Success();
    }

    public Result StartMaking(Guid correlationId)
    {
        if (this.Status != OrderStatus.Received)
        {
            return CommandResult.Fail(ResultStatus.BadRequest, "Only a received order can start being made.");
        }

        this.Status = OrderStatus.Making;

        this.DomainEvents.Raise(new OrderMakingStartedEvent(
            this.Id,
            this.UserId,
            DateTime.UtcNow,
            correlationId));

        return CommandResult.Success();
    }

    public Result Prepared(Guid correlationId)
    {
        if (!this.IsSubmitted)
        {
            return CommandResult.Fail(ResultStatus.BadRequest, "Cannot prepare an order that is not submitted.");
        }

        if (this.IsCompleted)
        {
            return CommandResult.Fail(ResultStatus.BadRequest, "Cannot prepare an order that is completed.");
        }

        if (this.IsPrepared)
        {
            return CommandResult.Success();
        }

        this.PreparedDateTime = DateTime.UtcNow;
        this.Status = this.IsDeliveryOrder ? OrderStatus.ReadyForDelivery : OrderStatus.ReadyForPickup;

        this.DomainEvents.Raise(new OrderPreparedEvent(
            this.Id,
            this.UserId,
            this.PreparedDateTime.Value,
            correlationId));

        return CommandResult.Success();
    }

    public Result OutForDelivery(Guid correlationId)
    {
        if (this.Status != OrderStatus.ReadyForDelivery)
        {
            return CommandResult.Fail(
                ResultStatus.BadRequest,
                "Only a prepared delivery order can be sent out for delivery.");
        }

        this.Status = OrderStatus.Delivering;

        this.DomainEvents.Raise(new OrderOutForDeliveryEvent(
            this.Id,
            this.UserId,
            DateTime.UtcNow,
            correlationId));

        return CommandResult.Success();
    }

    public Result PickedUp(Guid correlationId)
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
        this.Status = OrderStatus.Completed;

        this.DomainEvents.Raise(new OrderCompletedEvent(
            this.Id,
            this.UserId,
            this.CompletedDateTime.Value,
            correlationId));

        return CommandResult.Success();
    }

    public Result Delivered(Guid correlationId)
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
        this.Status = OrderStatus.Completed;

        this.DomainEvents.Raise(new OrderCompletedEvent(
            this.Id,
            this.UserId,
            this.CompletedDateTime.Value,
            correlationId));

        return CommandResult.Success();
    }

    private CommandResult EnsureCanSubmit()
    {
        if (this.IsSubmitted)
        {
            return CommandResult.Fail(ResultStatus.BadRequest, "Order has already been submitted.");
        }

        if (this.pizzas.Count == 0)
        {
            return CommandResult.Fail(ResultStatus.BadRequest, "Cannot submit an order with no pizzas.");
        }

        return CommandResult.Success();
    }

    public static class FieldLengths
    {
        public const int UserId = 100;
        public const int DeliveryAddress = 200;
    }
}
