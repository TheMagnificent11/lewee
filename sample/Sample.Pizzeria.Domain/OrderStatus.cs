namespace Sample.Pizzeria.Domain;

public enum OrderStatus
{
    New = 0,
    Submitted = 1,
    Prepared = 2,
    PickedUpForDelivery = 3,
    Delivered = 4
}
