using Lewee.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lewee.Infrastructure.Data;

/// <summary>
/// Base Enum Entity Configuration
/// </summary>
/// <typeparam name="TEnum">
/// Enum type
/// </typeparam>
public abstract class BaseEnumEntityConfiguration<TEnum> : IEntityTypeConfiguration<EnumEntity<TEnum>>
    where TEnum : struct, Enum
{
    /// <summary>
    /// Configures enum entity lookup database table
    /// </summary>
    /// <param name="builder">
    /// Table builder
    /// </param>
    public virtual void Configure(EntityTypeBuilder<EnumEntity<TEnum>> builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);
    }
}
