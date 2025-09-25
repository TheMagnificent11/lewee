using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Data.Configuration;

internal sealed class OrderPizzaConfiguration : RelationshipConfiguration<OrderPizza>
{
    protected override void ConfigureRelationship(EntityTypeBuilder<OrderPizza> builder)
    {
        builder
            .HasOne(op => op.Order)
            .WithMany(o => o.Pizzas)
            .HasForeignKey(op => op.OrderId);

        builder
            .HasOne(op => op.Pizza)
            .WithMany(p => p.OrderPizzas)
            .HasForeignKey(op => op.PizzaId);
    }
}
