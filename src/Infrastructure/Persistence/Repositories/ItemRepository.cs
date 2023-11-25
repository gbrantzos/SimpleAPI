using Microsoft.EntityFrameworkCore;
using SimpleAPI.Domain.Features.Items;

namespace SimpleAPI.Infrastructure.Persistence.Repositories;

public class ItemRepository : IItemRepository
{
    private readonly SimpleAPIContext _context;

    public ItemRepository(SimpleAPIContext context)
    {
        _context = context;
    }

    public void Add(Item item)
    {
        _context.Items.Add(item);
    }

    public async Task<Item?> GetByIDAsync(ItemID id, CancellationToken cancellationToken = default)
    {
        var result = await _context
            .Items
            .Include(i => i.Tags)
            .SingleOrDefaultAsync(i => i.ID == id, cancellationToken);
        return result;
    }

    public void Delete(Item item)
    {
        _context.Items.Remove(item);
    }
}
