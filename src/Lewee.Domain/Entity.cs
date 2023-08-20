namespace Lewee.Domain;

/// <summary>
/// Entity
/// </summary>
public abstract class Entity : ISoftDeleteEntity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Entity"/> class
    /// </summary>
    protected Entity()
        : this(Guid.NewGuid())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity"/> class
    /// </summary>
    /// <param name="id">ID</param>
    protected Entity(Guid id)
    {
        this.Id = id;

        // Note audit fields should be populated by EF save changes interceptor
        this.CreatedBy = "System";
        this.ModifiedBy = "System";
        this.CreatedAtUtc = DateTime.UtcNow;
        this.ModifiedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets or sets the ID of the entity
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// Gets the username of the user that created the entity
    /// </summary>
    public string CreatedBy { get; private set; }

    /// <summary>
    /// Gets the date-time in UTC at which the entity was created
    /// </summary>
    public DateTime CreatedAtUtc { get; private set; }

    /// <summary>
    /// Gets the username of the user that last modified the entity
    /// </summary>
    public string ModifiedBy { get; private set; }

    /// <summary>
    /// Gets the date-time in UTC at which the entity was last updated
    /// </summary>
    public DateTime ModifiedAtUtc { get; private set; }

    /// <inheritdoc />
    public bool IsDeleted { get; protected set; }

    /// <summary>
    /// Gets the timestamp
    /// </summary>
    public byte[] Timestamp { get; private set; } = default!;

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
        return (this.GetType().ToString() + this.Id.ToString())
            .GetHashCode();
    }

    /// <summary>
    /// Applies creation tracking data
    /// </summary>
    /// <param name="createdBy">Created by user</param>
    public void ApplyCreationTrackingData(string? createdBy)
    {
        this.CreatedBy = createdBy ?? "System";
        this.CreatedAtUtc = DateTime.UtcNow;
        this.ApplyModificationTrackingData(createdBy);
    }

    /// <summary>
    /// Applies modification tracking data
    /// </summary>
    /// <param name="modifiedBy">Modified by user</param>
    public void ApplyModificationTrackingData(string? modifiedBy)
    {
        this.ModifiedBy = modifiedBy ?? "System";
        this.ModifiedAtUtc = DateTime.UtcNow;
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
