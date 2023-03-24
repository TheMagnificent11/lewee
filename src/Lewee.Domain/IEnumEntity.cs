namespace Lewee.Domain
{
    /// <summary>
    /// Enum Entity Interface
    /// </summary>
    /// <typeparam name="TKey">
    /// Enum key type
    /// </typeparam>
    public interface IEnumEntity<TKey>
        where TKey : struct, IConvertible, IComparable
    {
        /// <summary>
        /// Gets the ID of the enum entity
        /// </summary>
        TKey Id { get; }

        /// <summary>
        /// Gets the enum item's name
        /// </summary>
        string Name { get; }
    }
}
