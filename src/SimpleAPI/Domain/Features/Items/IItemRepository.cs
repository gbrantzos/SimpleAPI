namespace SimpleAPI.Domain.Features.Items;

public interface IItemRepository
{
    Task<Item?> GetByID(int id, CancellationToken cancellationToken = default);
    
    Task Add(Item item, CancellationToken cancellationToken = default);
    
    void Delete(Item item);
}
