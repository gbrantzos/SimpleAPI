namespace SimpleAPI.Domain.Base;

public interface IRepository<TEntity, in TEntityID>
    where TEntity : Entity<TEntityID>
    where TEntityID : struct, IEntityID
{
    /// <summary>
    /// Get entity by ID (primary key). Returns null if no entity is found
    /// </summary>
    /// <param name="id">ID value (entity PK)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The requested entity or null if no entity found</returns>
    Task<TEntity?> GetByIDAsync(TEntityID id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add entity to repository
    /// </summary>
    /// <param name="entity">Entity instance to add</param>
    /// <returns></returns>
    void Add(TEntity entity);

    /// <summary>
    /// Mark entity for deletion
    /// </summary>
    /// <param name="entity">Entity instance to delete</param>
    void Delete(TEntity entity);

    /// <summary>
    /// Search for entities using the specified <see cref="SearchCriteria{T}"/>
    /// </summary>
    /// <param name="criteria">Query specification, details to include and sorting</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task<IReadOnlyList<TEntity>> FindAsync(SearchCriteria<TEntity> criteria, CancellationToken cancellationToken = default);

    /// <summary>
    /// Count entities that satisfy the given specification. Useful for paged results.
    /// </summary>
    /// <param name="specification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> CountAsync(Specification<TEntity> specification, CancellationToken cancellationToken = default);
}
