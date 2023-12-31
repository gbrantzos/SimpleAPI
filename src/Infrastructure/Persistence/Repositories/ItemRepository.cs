using SimpleAPI.Domain.Features.Items;
using SimpleAPI.Infrastructure.Persistence.Base;

namespace SimpleAPI.Infrastructure.Persistence.Repositories;

public class ItemRepository : BaseRepository<Item, ItemID>, IItemRepository
{
    protected override IEnumerable<string> DefaultDetails => new[]
    {
        nameof(Item.Features),
        nameof(Item.AlternativeCodes)
    };

    public ItemRepository(SimpleAPIContext context) : base(context) { }
}
