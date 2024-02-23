using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.Pizzeria.Domain;

namespace Sample.Pizzeria.Infrastructure.Data.Configuration;

public sealed class OrderPizzaConfiguration : EntityConfiguration<OrderPizza>
{
    protected override void ConfigureEntity(EntityTypeBuilder<OrderPizza> builder)
    {
        builder.ToTable("OrderPizzas");

        builder.HasOne(x => x.Order)
            .WithMany(x => x.Pizzas)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
