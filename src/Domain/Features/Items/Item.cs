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

    public void AddTag(Tag tag)
    {
        var existing = _tags.FirstOrDefault(t => t.ID == tag.ID);
        if (existing is null)
        {
            _tags.Add(tag);
            return;
        }
        
        // Tag exists, do whatever you like (update maybe??)
        existing.Name = tag.Name;
    }
}

[HasStronglyTypedID]
public class Tag : Entity<TagID>
{
    public string Name { get; set; } = String.Empty;
}
