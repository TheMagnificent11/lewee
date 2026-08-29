namespace Lewee.Domain;

/// <summary>
/// Entity
/// </summary>
public abstract class Entity : AuditableRecord, ISoftDeleteEntity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Entity"/> class
    /// </summary>
    protected Entity()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity"/> class
    /// </summary>
    /// <param name="id">ID</param>
    protected Entity(Guid id)
        : base(id)
    {
    }

    /// <inheritdoc />
    public bool IsDeleted { get; protected set; }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
        {
            return false;
        }

        if (this.GetType() != other.GetType())
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.Id == other.Id;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return StringComparer.Ordinal.GetHashCode(this.GetType().ToString() + this.Id.ToString());
    }

    /// <summary>
    /// Marks entity as soft-deleted
    /// </summary>
    public void Delete()
    {
        if (this.IsDeleted)
        {
            return;
        }

        this.IsDeleted = true;
    }

    /// <summary>
    /// Marks entity as not soft-deleted
    /// </summary>
    public void Undelete()
    {
        if (!this.IsDeleted)
        {
            return;
        }

        this.IsDeleted = false;
    }
}
