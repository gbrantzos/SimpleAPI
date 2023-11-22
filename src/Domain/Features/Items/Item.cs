using SimpleAPI.Core;
using SimpleAPI.Domain.Base;

namespace SimpleAPI.Domain.Features.Items;

[HasStronglyTypedID]
public class Item : Entity<ItemID>, IVersioned
{
    public int RowVersion { get; set; }
    public string Code { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
}

[HasStronglyTypedID]
public class Tag : Entity<TagID>
{
    public string Name { get; set; } = String.Empty;
}
