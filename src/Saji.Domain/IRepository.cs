namespace Saji.Domain;

/// <summary>
/// Repository interface to access data of <typeparamref name="T"/>
/// </summary>
/// <typeparam name="T">Data type to access</typeparam>
public interface IRepository<T>
    where T : class, IEntity
{
    /// <summary>
    /// Retrieves the entity with ID <paramref name="id"/>
    /// </summary>
    /// <param name="id">ID of entity to retrieve</param>
    /// <returns>An async task that contains the entity if it exist, otherwise the task contains null</returns>
    Task<T?> RetrieveById(Guid id);

    /// <summary>
    /// Gets an <see cref="IQueryable{T}"/> of all entities
    /// </summary>
    /// <returns><see cref="IQueryable{T}"/> of all entities</returns>
    IQueryable<T> All();
}
