namespace Lewee.Domain;

/// <summary>
/// Base Entity
/// </summary>
public abstract class BaseEntity : IEntity, ISoftDeleteEntity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEntity"/> class
    /// </summary>
    protected BaseEntity()
        : this(Guid.NewGuid())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEntity"/> class
    /// </summary>
    /// <param name="id">ID</param>
    protected BaseEntity(Guid id)
    {
        this.Id = id;

        // Note audit fields should be popualated by EF save changes interceptor
        this.CreatedBy = "System";
        this.ModifiedBy = "System";
        this.CreatedAtUtc = DateTime.UtcNow;
        this.ModifiedAtUtc = DateTime.UtcNow;
    }

    /// <inheritdoc />
    public Guid Id { get; protected set; }

    /// <inheritdoc />
    public string CreatedBy { get; private set; }

    /// <inheritdoc />
    public DateTime CreatedAtUtc { get; private set; }

    /// <inheritdoc />
    public string ModifiedBy { get; private set; }

    /// <inheritdoc />
    public DateTime ModifiedAtUtc { get; private set; }

    /// <inheritdoc />
    public bool IsDeleted { get; protected set; }

    /// <inheritdoc />
    public byte[] Timestamp { get; protected set; } = default!;

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is not BaseEntity other)
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
