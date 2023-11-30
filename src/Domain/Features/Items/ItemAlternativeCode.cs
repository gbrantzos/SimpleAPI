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
    
    public ItemCode Code { get; }
    public CodeKind Kind { get; }
    public string? Description { get; set; }

    public ItemAlternativeCode(ItemCode code, CodeKind kind)
    {
        Code = code;
        Kind = kind;
    }
}
