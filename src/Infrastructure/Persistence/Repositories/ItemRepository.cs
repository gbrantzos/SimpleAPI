using Microsoft.EntityFrameworkCore;
using SimpleAPI.Core;
using SimpleAPI.Domain.Features.Items;

namespace SimpleAPI.Infrastructure.Persistence.Repositories;

public class ItemRepository : IItemRepository
{
    private readonly SimpleAPIContext _context;

    public ItemRepository(SimpleAPIContext context)
    {
        _context = context.ThrowIfNull();
    }

    public void Add(Item entity)
    {
        _context.Items.Add(entity);
    }

    public async Task<Item?> GetByIDAsync(ItemID id, CancellationToken cancellationToken = default)
    {
        var result = await _context
            .Items
            .Include(i => i.Tags)
            .SingleOrDefaultAsync(i => i.ID == id, cancellationToken);
        return result;
    }

    public void Delete(Item entity)
    {
        _context.Items.Remove(entity);
    }
}
