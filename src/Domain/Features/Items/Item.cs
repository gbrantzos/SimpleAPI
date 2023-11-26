using SimpleAPI.Core;
using SimpleAPI.Domain.Base;
using SimpleAPI.Domain.Features.Common;

namespace SimpleAPI.Domain.Features.Items;

[HasStronglyTypedID]
public class Item : Entity<ItemID>, IVersioned
{
    private readonly List<Tag> _tags = new List<Tag>();

    public int RowVersion { get; set; }
    public string Code { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public Money Price { get; set; } = Money.InEuro(0);
    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

    public override string ToString() => $"Item with ID {ID}";

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

[HasStronglyTypedID]
public class Tag : Entity<TagID>
{
    public Tag(string name) => Name = name;
    
    public string Name { get; init; }

    public override string ToString() => $"{Name} [ID {ID}]";
}
