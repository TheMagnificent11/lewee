namespace Lewee.Domain
{
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

            this.DomainEvents = new DomainEventsCollection();
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

        /// <summary>
        /// Gets or sets a value indicating whether the entity is deleted
        /// </summary>
        public bool IsDeleted { get; protected set; }

        /// <summary>
        /// Gets or sets the timestamp
        /// </summary>
        public byte[] Timestamp { get; protected set; } = default!;

        /// <summary>
        /// Gets or sets the domain events collection
        /// </summary>
        public DomainEventsCollection DomainEvents { get; protected set; }

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
    }
}
