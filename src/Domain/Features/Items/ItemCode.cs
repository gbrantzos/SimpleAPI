using SimpleAPI.Core.Guards;

namespace SimpleAPI.Domain.Features.Items;

public readonly struct ItemCode : IEquatable<ItemCode>, IComparable<ItemCode>
{
    private readonly string _value;

    private ItemCode(string code)
    {
        Ensure.NotNull(code);
        Ensure.Applies(code, (v) => v.Length is >= 3 and <= 50, "Code length must be between 3 and 50 characters");
        _value = code;
    }

    public override bool Equals(object? obj) => obj is ItemCode other && Equals(other);
    public int CompareTo(ItemCode other) => String.Compare(_value, other._value, StringComparison.OrdinalIgnoreCase);

    public bool Equals(ItemCode other) => _value == other._value;

    public override int GetHashCode() => _value.GetHashCode(StringComparison.InvariantCultureIgnoreCase);
    public override string ToString() => _value;

    public static bool operator ==(ItemCode a, ItemCode b) => a.CompareTo(b) == 0;
    public static bool operator !=(ItemCode a, ItemCode b) => !(a == b);

    public static bool operator <(ItemCode left, ItemCode right) => left.CompareTo(right) < 0;
    public static bool operator <=(ItemCode left, ItemCode right) => left.CompareTo(right) <= 0;
    public static bool operator >(ItemCode left, ItemCode right) => left.CompareTo(right) > 0;
    public static bool operator >=(ItemCode left, ItemCode right) => left.CompareTo(right) >= 0;

    public static implicit operator string(ItemCode code) => code._value;
    public static explicit operator ItemCode(string code) => new ItemCode(code);

    public static ItemCode FromString(string code) => (ItemCode)code;
}
