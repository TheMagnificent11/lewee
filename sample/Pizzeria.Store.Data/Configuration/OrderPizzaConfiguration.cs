using System.Diagnostics.CodeAnalysis;
using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Data.Configuration;

[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes",
    Justification = "Used via assembly-scanning")]
internal sealed class OrderPizzaConfiguration : RelationshipConfiguration<OrderPizza>
{
    protected override void ConfigureRelationship(EntityTypeBuilder<OrderPizza> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

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
