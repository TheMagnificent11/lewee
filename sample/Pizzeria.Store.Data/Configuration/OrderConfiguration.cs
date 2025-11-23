using System.Diagnostics.CodeAnalysis;
using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Data.Configuration;

[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes",
    Justification = "Used via assembly-scanning")]
internal sealed class OrderConfiguration : AggregateRootConfiguration<Order>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Order> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(Order.FieldLengths.UserId);

        builder
            .Property(x => x.DeliveryAddress)
            .HasMaxLength(Order.FieldLengths.DeliveryAddress);

        builder
            .HasMany(o => o.Pizzas)
            .WithOne(op => op.Order)
            .HasForeignKey(op => op.OrderId);
    }
}
