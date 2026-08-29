using System.Diagnostics.CodeAnalysis;
using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Data.Configuration;

[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes",
    Justification = "Used via assembly-scanning")]
internal sealed class PizzaConfiguration : AggregateRootConfiguration<Pizza>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Pizza> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(Pizza.FieldLengths.Name);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(Pizza.FieldLengths.Description);

        builder.Property(x => x.Price)
            .IsRequired()
            .HasPrecision(5, 2);
    }
}
