using Lewee.Domain;

namespace Sample.Pizzeria.Domain;

public class Order : AggregateRoot
{
    public const int DeliveryAddressMaxLength = 255;

    private readonly List<OrderPizza> pizzas = [];

    public Order(Guid id, string userId, Guid correlationId)
        : base(id)
    {
        this.UserId = userId;
        this.StatusId = OrderStatus.New;

        this.DomainEvents.Raise(new OrderCreatedDomainEvent(this.Id, correlationId));
    }

    private Order()
    {
        // Required by EF Core
    }

    public string UserId { get; protected set; }

    public DateTime OrderPlacedDateTime { get; protected set; }

    public string? DeliveryAddress { get; protected set; }

    public IReadOnlyCollection<OrderPizza> Pizzas
    {
        get
        {
            return this.pizzas;
        }

        protected set
        {
            this.pizzas.Clear();
            this.pizzas.AddRange(value);
        }
    }

    public OrderStatus StatusId { get; protected set; }

    public EnumEntity<OrderStatus> Status { get; protected set; }

    public DateTime? PreparedDateTime { get; protected set; }

    public DateTime? PickedUpForDeliveryDateTime { get; protected set; }

    public DateTime? DeliveredDateTime { get; protected set; }

    public void AddPizza(Pizza pizza, Guid correlationId)
    {
        if (this.StatusId != OrderStatus.New)
        {
            throw new Exception($"Invalid order status {this.Status.Name}");
        }

        var orderPizza = this.pizzas.SingleOrDefault(p => p.PizzaId == pizza.Id);

        if (orderPizza == null)
        {
            orderPizza = new OrderPizza(this, pizza.Id);
            this.pizzas.Add(orderPizza);
        }
        else
        {
            orderPizza.IncreaseQuantity(1);
        }

        this.DomainEvents.Raise(new PizzaAddedToOrderDomainEvent(this.Id, orderPizza.Id, correlationId));
    }

    public void RemovePizza(Pizza pizza, Guid correlationId)
    {
        if (this.StatusId != OrderStatus.New)
        {
            throw new Exception($"Invalid order status {this.Status.Name}");
        }

        var orderPizza = this.pizzas.SingleOrDefault(p => p.PizzaId == pizza.Id)
            ?? throw new DomainException("Pizza not found in order");

        orderPizza.DecreaseQuantity(1);

        this.DomainEvents.Raise(new PizzaRemovedFromOrderDomainEvent(this.Id, orderPizza.Id, correlationId));
    }

    public void Place(string deliveryAddress, Guid correlationId)
    {
        if (this.StatusId != OrderStatus.New)
        {
            throw new Exception($"Invalid order status {this.Status.Name}");
        }

        this.StatusId = OrderStatus.Submitted;
        this.OrderPlacedDateTime = DateTime.UtcNow;
        this.DeliveryAddress = deliveryAddress;

        this.DomainEvents.Raise(new OrderPlacedDomainEvent(this.Id, correlationId));
    }

    public void Prepare(Guid correlationId)
    {
        switch (this.StatusId)
        {
            case OrderStatus.Submitted:
                break;

            case OrderStatus.Prepared:
                return;

            default:
                throw new Exception($"Invalid order status {this.Status.Name}");
        }

        this.StatusId = OrderStatus.Prepared;
        this.PreparedDateTime = DateTime.UtcNow;

        this.DomainEvents.Raise(new OrderPreparedDomainEvent(this.Id, correlationId));
    }

    public void PickUpForDelivery(Guid correlationId)
    {
        switch (this.StatusId)
        {
            case OrderStatus.Prepared:
                break;

            case OrderStatus.PickedUpForDelivery:
                return;

            default:
                throw new Exception($"Invalid order status {this.Status.Name}");
        }

        this.StatusId = OrderStatus.PickedUpForDelivery;
        this.PickedUpForDeliveryDateTime = DateTime.UtcNow;

        this.DomainEvents.Raise(new OrderPickedUpForDeliveryDomainEvent(this.Id, correlationId));
    }

    public void Deliver(Guid correlationId)
    {
        switch (this.StatusId)
        {
            case OrderStatus.PickedUpForDelivery:
                break;

            case OrderStatus.Delivered:
                return;

            default:
                throw new Exception($"Invalid order status {this.Status.Name}");
        }

        this.StatusId = OrderStatus.Delivered;
        this.DeliveredDateTime = DateTime.UtcNow;

        this.DomainEvents.Raise(new OrderDeliverDomainEvent(this.Id, correlationId));
    }
}
