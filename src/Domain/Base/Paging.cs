namespace SimpleAPI.Domain.Base;

public abstract class Paging
{
    public abstract RowBounds GetRowBounds();
}

public sealed class OffsetBasedPaging : Paging
{
    public int Offset { get; init; }
    public int Limit { get; init; }

    public override RowBounds GetRowBounds() => new() { First = Offset, Total = Limit };
}

public sealed class AbsolutePaging : Paging
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public override RowBounds GetRowBounds() => new() { First = (PageNumber - 1) * PageSize + 1, Total = PageSize };
}

public readonly struct RowBounds : IEquatable<RowBounds>
{
    public int First { get; init; }
    public int Total { get; init; }


    public override bool Equals(object? obj) => obj is RowBounds other && Equals(other);

    public bool Equals(RowBounds other) => First == other.First && Total == other.Total;

    public override int GetHashCode() => HashCode.Combine(First, Total);

    public static bool operator ==(RowBounds left, RowBounds right) => left.Equals(right);

    public static bool operator !=(RowBounds left, RowBounds right) => !(left == right);
}
