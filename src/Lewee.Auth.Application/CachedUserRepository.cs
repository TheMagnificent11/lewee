using Lewee.Auth.Domain;
using Lewee.Domain;
using Microsoft.Extensions.Caching.Memory;

namespace Lewee.Auth.Application;

/// <summary>
/// Decorates an <see cref="IRepository{T}"/> of <see cref="User"/> with an in-memory cache of lookups by
/// external identity (via <see cref="UserByExternalIdSpecification"/>), avoiding a database round trip for
/// every authorization check against the same user.
/// </summary>
internal sealed class CachedUserRepository : IRepository<User>
{
    // Set(key, value, TimeSpan) is an absolute expiration relative to now, not a sliding window - an entry
    // is evicted 1 hour after being cached, regardless of how frequently it is accessed in the meantime.
    private readonly IRepository<User> innerRepository;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachedUserRepository"/> class.
    /// </summary>
    /// <param name="innerRepository">The decorated user repository.</param>
    /// <param name="cache">Memory cache.</param>
    public CachedUserRepository(IRepository<User> innerRepository, IMemoryCache cache)
    {
        this.innerRepository = innerRepository;
        this.cache = cache;
    }

    internal static TimeSpan CacheDuration { get; set; } = TimeSpan.FromHours(1);

    /// <inheritdoc />
    public async Task<User?> RetrieveByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = BuildIdCacheKey(id);

        if (this.cache.TryGetValue(cacheKey, out User? cachedUser))
        {
            return cachedUser;
        }

        var user = await this.innerRepository.RetrieveByIdAsync(id, cancellationToken);
        this.cache.Set(cacheKey, user, CacheDuration);

        return user;
    }

    /// <inheritdoc />
    public Task<List<User>> AllAsync(CancellationToken cancellationToken = default) =>
        this.innerRepository.AllAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<List<User>> QueryAsync(
        QuerySpecification<User> querySpecification,
        CancellationToken cancellationToken = default) =>
        await this.innerRepository.QueryAsync(querySpecification, cancellationToken);

    /// <inheritdoc />
    public async Task<User?> QueryOneAsync(
        QuerySpecification<User> querySpecification,
        CancellationToken cancellationToken = default)
    {
        if (querySpecification is not UserByExternalIdSpecification externalIdSpecification)
        {
            return await this.innerRepository.QueryOneAsync(querySpecification, cancellationToken);
        }

        var cacheKey = BuildExternalIdCacheKey(externalIdSpecification.ExternalId);

        if (this.cache.TryGetValue(cacheKey, out User? cachedUser))
        {
            return cachedUser;
        }

        var user = await this.innerRepository.QueryOneAsync(querySpecification, cancellationToken);
        this.cache.Set(cacheKey, user, CacheDuration);

        return user;
    }

    /// <inheritdoc />
    public Task AddAsync(User entity, CancellationToken cancellationToken = default) =>
        this.innerRepository.AddAsync(entity, cancellationToken);

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        this.innerRepository.SaveChangesAsync(cancellationToken);

    private static string BuildIdCacheKey(Guid id) => $"{nameof(CachedUserRepository)}:Id:{id}";

    private static string BuildExternalIdCacheKey(string externalId) =>
        $"{nameof(CachedUserRepository)}:ExternalId:{externalId}";
}
