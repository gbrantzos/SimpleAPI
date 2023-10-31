using SimpleAPI.Domain.Features.Items;

namespace SimpleAPI.Infrastructure.Persistence.Repositories;

public class ItemRepository : IItemRepository
{
    private readonly SimpleAPIContext _context;

    public ItemRepository(SimpleAPIContext context)
    {
        _context = context;
    }

    public async Task Add(Item item, CancellationToken cancellationToken = default)
    {
        await _context.Items.AddAsync(item, cancellationToken);
    }

    public async Task<Item?> GetByID(int id, CancellationToken cancellationToken = default)
    {
        var result = await _context.Items.FindAsync(new object[] { id }, cancellationToken);
        return result;
    }

    public void Delete(Item item)
    {
        _context.Items.Remove(item);
    }
}
