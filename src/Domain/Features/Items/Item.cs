using SimpleAPI.Core;
using SimpleAPI.Domain.Base;
using SimpleAPI.Domain.Features.Common;

namespace SimpleAPI.Domain.Features.Items;

[StronglyTypedID]
public class Item : Entity<ItemID>, IVersioned, IAuditable
{
    private readonly List<Tag> _tags = new List<Tag>();
    private readonly List<ItemAlternativeCode> _alternativeCodes = new();

    public int RowVersion { get; set; }
    public ItemCode Code { get; private set; }
    public string Description { get; set; }
    public Money Price { get; private set; } = Money.InEuro(0);
    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();
    public IReadOnlyCollection<ItemAlternativeCode> AlternativeCodes => _alternativeCodes.AsReadOnly();
    
    private Item(ItemCode code, string description)
    {
        Code        = code;
        Description = description;
    }
    
    public static Item Create(string code, string description)
    {
        var newItem = new Item((ItemCode)code, description);
        
        var alternativeCode = new ItemAlternativeCode((ItemCode)code, ItemAlternativeCode.CodeKind.Base);
        newItem._alternativeCodes.Add(alternativeCode);
        
        return newItem;
    }

    public override string ToString() => $"Item with ID {ID}";

    public void SetPrice(Money price) => Price = price;
    
    public void AddTag(Tag tag)
    {
        var existing = _tags.FirstOrDefault(t => t.Name == tag.Name);
        if (existing is null)
        {
            _tags.Add(tag);
        }
    }

    public void RemoveTag(Tag tag)
    {
        var existing = _tags.FirstOrDefault(t => t.Name == tag.Name);
        if (existing is not null)
        {
            _tags.Remove(existing);
        }
    }

    public void AddCode(ItemCode code)
    {
        var existing = _alternativeCodes
            .FirstOrDefault(c => c.Code == code && c.Kind == ItemAlternativeCode.CodeKind.Alternative);
        if (existing is not null)
            return;
        
        var alternativeCode = new ItemAlternativeCode(code, ItemAlternativeCode.CodeKind.Alternative);
        _alternativeCodes.Add(alternativeCode);
    }

    public void RemoveCode(ItemCode code)
    {
        var existing = _alternativeCodes
            .FirstOrDefault(c => c.Code == code && c.Kind == ItemAlternativeCode.CodeKind.Alternative);
        if (existing is not null)
        {
            _alternativeCodes.Remove(existing);
        }
    }
}

[StronglyTypedID]
public class Tag : Entity<TagID>
{
    public Tag(string name) => Name = name;
    
    public string Name { get; init; }

    public override string ToString() => $"{Name} [ID {ID}]";
}
