using SimpleAPI.Core;
using SimpleAPI.Domain.Base;
using SimpleAPI.Domain.Features.Common;

namespace SimpleAPI.Domain.Features.Items;

[StronglyTypedID]
public class Item : Entity<ItemID>, IVersioned, IAuditable, ISoftDelete
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

    public bool ChangeCode(string code)
    {
        var isUsed = _alternativeCodes
            .Any(c => c.Code == code && c.Kind == ItemAlternativeCode.CodeKind.Alternative);
        if (isUsed) return false;

        var itemCode = (ItemCode)code;
        Code = itemCode;

        var existing = _alternativeCodes.Single(c => c.Kind == ItemAlternativeCode.CodeKind.Base);
        _alternativeCodes.Remove(existing);
        _alternativeCodes.Add(new ItemAlternativeCode(itemCode, ItemAlternativeCode.CodeKind.Base));

        return true;
    }

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

    public void AddAlternativeCode(string code, string? description = null)
    {
        var existing = GetAlternativeCode(code);
        if (existing is not null)
            return;

        var alternativeCode = new ItemAlternativeCode((ItemCode)code, ItemAlternativeCode.CodeKind.Alternative)
        {
            Description = description
        };
        _alternativeCodes.Add(alternativeCode);
    }

    public void RemoveAlternativeCode(string code)
    {
        var existing = _alternativeCodes
            .FirstOrDefault(c => c.Code == code && c.Kind == ItemAlternativeCode.CodeKind.Alternative);
        if (existing is not null)
        {
            _alternativeCodes.Remove(existing);
        }
    }

    public ItemAlternativeCode? GetAlternativeCode(string code)
        => _alternativeCodes.FirstOrDefault(c => c.Code == code && c.Kind == ItemAlternativeCode.CodeKind.Alternative);
}
