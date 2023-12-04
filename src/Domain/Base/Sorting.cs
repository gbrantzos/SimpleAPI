using System.Linq.Expressions;

namespace SimpleAPI.Domain.Base;

public abstract class Sorting
{
    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public SortDirection Direction { get; protected init; }
}

public class Sorting<T> : Sorting
{
    public Expression<Func<T, object>>? OrderBy { get; }

    public Sorting(Expression<Func<T, object>>? orderBy, SortDirection direction = SortDirection.Ascending)
    {
        OrderBy   = orderBy;
        Direction = direction;
    }
}
