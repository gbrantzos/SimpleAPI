using SimpleAPI.Core.Base;
using SimpleAPI.Core.Guards;

namespace SimpleAPI.Domain.Features.Items;

public sealed class ItemCode : ValueObject
{
    private readonly string _value;

    private ItemCode(string code)
    {
        Ensure.NotNull(code);
        Ensure.Applies(code, (v) => v.Length is >= 3 and <= 50, "Code length must be between 3 and 50 characters");
        _value = code;
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return _value;
    }

    public override string ToString() => _value;

    public static implicit operator string(ItemCode code) => code._value;
    public static explicit operator ItemCode(string code) => new ItemCode(code);

    public static ItemCode FromString(string code) => (ItemCode)code;
    public static string ToString(ItemCode code) => code;
}
