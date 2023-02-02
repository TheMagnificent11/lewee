using Lewee.Shared;

namespace Lewee.Domain;

/// <summary>
/// Enum Entity
/// </summary>
/// <typeparam name="TKey">
/// Enum entity key type
/// </typeparam>
public class EnumEntity<TKey> : IEnumEntity<TKey>
    where TKey : struct, Enum
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnumEntity{TKey}"/> class
    /// </summary>
    /// <param name="id">
    /// ID
    /// </param>
    public EnumEntity(TKey id)
    {
        this.Id = id;
        this.Name = id.GetDescription();
    }

    // EF Core constructor
    private EnumEntity()
    {
        this.Name = "EF";
    }

    /// <inheritdoc />
    public TKey Id { get; protected set; }

    /// <inheritdoc />
    public string Name { get; protected set; }
}
