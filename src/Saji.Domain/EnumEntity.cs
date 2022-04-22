namespace Saji.Domain
{
    /// <summary>
    /// Enum Entity
    /// </summary>
    /// <typeparam name="TKey">
    /// Enum entity key type
    /// </typeparam>
    public class EnumEntity<TKey> : IEnumEntity<TKey>
        where TKey : struct, IConvertible, IComparable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumEntity{TKey}"/> class
        /// </summary>
        /// <param name="id">
        /// ID
        /// </param>
        /// <param name="name">
        /// Name
        /// </param>
        protected EnumEntity(TKey id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the ID of the enum entity
        /// </summary>
        public TKey Id { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the enum entity
        /// </summary>
        public string Name { get; protected set; }
    }
}
