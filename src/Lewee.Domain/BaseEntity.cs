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
        {
            var currentUser = GetCurrentUserName();

            this.Id = Guid.NewGuid();
            this.CreatedBy = currentUser;
            this.CreatedAtUtc = DateTime.UtcNow;
            this.ModifiedBy = currentUser;
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
        /// Applies the tracking data
        /// </summary>
        public void ApplyTrackingData()
        {
            var userName = Thread.CurrentPrincipal?.Identity?.Name ?? "System";
            var now = DateTime.UtcNow;

            if (string.IsNullOrEmpty(this.CreatedBy))
            {
                this.CreatedBy = userName;
                this.CreatedAtUtc = now;
            }

            this.ModifiedBy = userName;
            this.ModifiedAtUtc = now;
        }

        private static string GetCurrentUserName()
        {
            return Environment.UserName ?? "System";
        }
    }
}
