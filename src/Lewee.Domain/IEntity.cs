using System;

namespace Lewee.Domain
{
    /// <summary>
    /// Entity Interface
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Gets the ID of the entity
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the username of the user that created the entity
        /// </summary>
        string CreatedBy { get; }

        /// <summary>
        /// Gets the date-time in UTC at which the entity was created
        /// </summary>
        DateTime CreatedAtUtc { get; }

        /// <summary>
        /// Gets the username of the user that last modified the entity
        /// </summary>
        string ModifiedBy { get; }

        /// <summary>
        /// Gets the date-time in UTC at which the entity was last updated
        /// </summary>
        DateTime ModifiedAtUtc { get; }

        /// <summary>
        /// Gets the timestamp
        /// </summary>
        byte[] Timestamp { get; }
    }
}
