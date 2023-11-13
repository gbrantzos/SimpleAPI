using SimpleAPI.Domain.Base;

namespace SimpleAPI.Domain.Features.Items;

public class ItemID : EntityID
{
    public ItemID(int id) : base(id) { }
    public ItemID() : base(0) { }
}

public class Item : Entity<ItemID>
{
    public string Code { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
}
