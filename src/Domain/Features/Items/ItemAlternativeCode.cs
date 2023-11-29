using SimpleAPI.Core;
using SimpleAPI.Domain.Base;

namespace SimpleAPI.Domain.Features.Items;

[StronglyTypedID]
public class ItemAlternativeCode : Entity<ItemAlternativeCodeID>, IAuditable
{
    public enum CodeKind
    {
        Base = 0,
        Alternative = 1
    }
    
    public ItemCode Code { get; set; }
    public CodeKind Kind { get; set; }

    public ItemAlternativeCode(ItemCode code, CodeKind kind)
    {
        Code = code;
        Kind = kind;
    }
}
