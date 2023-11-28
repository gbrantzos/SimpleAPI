using SimpleAPI.Core;
using SimpleAPI.Domain.Base;
using SimpleAPI.Domain.Features.Common;

namespace SimpleAPI.Domain.Features.Items;

[StronglyTypedID]
public class Item : Entity<ItemID>, IVersioned
{
    private readonly List<Tag> _tags = new List<Tag>();

    public int RowVersion { get; set; }
    public string Code { get; private set; }
    public string Description { get; set; }
    public Money Price { get; private set; } = Money.InEuro(0);
    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

    private Item(string code, string description)
    {
        Code        = code;
        Description = description;
    }
    
    public static Item Create(string code, string description)
    {
        var newItem = new Item(code, description);
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
}

[StronglyTypedID]
public class Tag : Entity<TagID>
{
    public Tag(string name) => Name = name;
    
    public string Name { get; init; }

    public override string ToString() => $"{Name} [ID {ID}]";
}
