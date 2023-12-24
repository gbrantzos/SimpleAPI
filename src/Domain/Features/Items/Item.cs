using SimpleAPI.Core;
using SimpleAPI.Core.Guards;
using SimpleAPI.Domain.Base;
using SimpleAPI.Domain.Features.Common;

namespace SimpleAPI.Domain.Features.Items;

[StronglyTypedID]
public class Item : Entity<ItemID>, IVersioned, IAuditable, ISoftDelete
{
    private readonly List<Feature> _features = new();
    private readonly List<ItemAlternativeCode> _alternativeCodes = new();

    public int RowVersion { get; set; }
    public ItemCode Code { get; private set; }
    public string Description { get; set; }
    public Money Price { get; private set; } = Money.InEuro(0);
    public IReadOnlyList<Feature> Features => _features.ToList();
    public IReadOnlyList<ItemAlternativeCode> AlternativeCodes => _alternativeCodes.ToList();

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

    public void AddFeature(Feature attribute)
    {
        var existing = _features.FirstOrDefault(t => t.Name == attribute.Name);
        if (existing is null)
        {
            _features.Add(attribute);
        }
    }

    public void RemoveFeature(Feature attribute)
    {
        Ensure.NotNull(attribute);
        var existing = _features.FirstOrDefault(t => t.Name == attribute.Name);
        if (existing is not null)
        {
            _features.Remove(existing);
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

public class ItemFeature : Entity, IAuditable
{
    public ItemID ItemID { get; set; }
    public FeatureID FeatureID { get; set; }
}
