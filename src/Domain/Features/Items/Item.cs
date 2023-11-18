using SimpleAPI.Core;
using SimpleAPI.Domain.Base;

namespace SimpleAPI.Domain.Features.Items;

[HasStronglyTypedID]
public class Item : Entity<ItemID>
{
    public string Code { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
}
