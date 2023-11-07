namespace SimpleAPI.Domain.Features.Items;

public interface IItemRepository
{
    /// <summary>
    /// Get entity by ID (primary key). Returns null if no entity is found
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Item?> GetByIDAsync(int id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Add entity to repository for saving
    /// </summary>
    /// <param name="item"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddAsync(Item item, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Mark entity for deletion
    /// </summary>
    /// <param name="item"></param>
    void Delete(Item item);
}
