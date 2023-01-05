namespace Lewee.Domain
{
    /// <summary>
    /// Soft Delete Entity Interface
    /// </summary>
    public interface ISoftDeleteEntity
    {
        /// <summary>
        /// Gets a value indicating whether the entity is deleted
        /// </summary>
        bool IsDeleted { get; }
    }
}
