namespace Lewee.Domain;

/// <summary>
/// Auditable Record Interface
/// </summary>
public abstract class AuditableRecord
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuditableRecord"/> class
    /// </summary>
    protected AuditableRecord()
    {
        // Note audit fields should be populated by EF save changes interceptor
        this.CreatedBy = "System";
        this.ModifiedBy = "System";
        this.CreatedAtUtc = DateTime.UtcNow;
        this.ModifiedAtUtc = DateTime.UtcNow;
    }

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

    /// <summary>
    /// Applies creation tracking data to the entity
    /// </summary>
    /// <param name="createdBy">Created by user ID</param>
    public void ApplyCreationTrackingData(string? createdBy)
    {
        this.CreatedBy = createdBy ?? "System";
        this.CreatedAtUtc = DateTime.UtcNow;
        this.ApplyModificationTrackingData(createdBy);
    }

    /// <summary>
    /// Applies modification tracking data to the entity
    /// </summary>
    /// <param name="modifiedBy">Modified by user ID</param>
    public void ApplyModificationTrackingData(string? modifiedBy)
    {
        this.ModifiedBy = modifiedBy ?? "System";
        this.ModifiedAtUtc = DateTime.UtcNow;
    }
}
