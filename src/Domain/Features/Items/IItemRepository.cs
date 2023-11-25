using SimpleAPI.Domain.Base;

namespace SimpleAPI.Domain.Features.Items;

public interface IItemRepository : IRepository<Item, ItemID> { }
