namespace SimpleAPI.Domain.Base;

public class QueryResult<T>
{
    public IReadOnlyCollection<T> Items { get; set; }
    public long Count { get; set; }

    public QueryResult(IReadOnlyCollection<T> items)
    {
        Items = items;
        Count = items.Count;
    }
}
