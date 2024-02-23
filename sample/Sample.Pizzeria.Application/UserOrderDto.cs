namespace Sample.Pizzeria.Application;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
public record UserOrderDto(Guid OrderId, string Status);
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
