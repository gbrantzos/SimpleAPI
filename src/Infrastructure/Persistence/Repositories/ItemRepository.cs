using SimpleAPI.Domain.Features.Items;
using SimpleAPI.Infrastructure.Persistence.Base;

namespace SimpleAPI.Infrastructure.Persistence.Repositories;

public class ItemRepository : BaseRepository<Item, ItemID>, IItemRepository
{
    protected override IReadOnlyCollection<string> DefaultDetails => new[] { "Tags" };

    public ItemRepository(SimpleAPIContext context) : base(context) { }
}
