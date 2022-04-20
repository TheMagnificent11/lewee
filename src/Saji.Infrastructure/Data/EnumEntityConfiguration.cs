using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Saji.Domain;

namespace Saji.Infrastructure.Data;

/// <summary>
/// <see cref="IEnumEntity{TKey}"/> Configuration
/// </summary>
/// <typeparam name="TEnum">
/// Enum type
/// </typeparam>
/// <typeparam name="TEnumEntity">
/// Enum lookup table type
/// </typeparam>
public class EnumEntityConfiguration<TEnum, TEnumEntity> : IEntityTypeConfiguration<TEnumEntity>
    where TEnum : struct, IConvertible, IComparable
    where TEnumEntity : class, IEnumEntity<TEnum>
{
    /// <summary>
    /// Configures enum entity lookup database table
    /// </summary>
    /// <param name="builder">
    /// Table builder
    /// </param>
    public virtual void Configure(EntityTypeBuilder<TEnumEntity> builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .ValueGeneratedNever();

        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(100);
    }
}
