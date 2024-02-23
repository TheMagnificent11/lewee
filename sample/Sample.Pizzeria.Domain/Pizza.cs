namespace Sample.Pizzeria.Domain;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
public record Pizza(int Id, string Name, string Description, decimal Price);
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
