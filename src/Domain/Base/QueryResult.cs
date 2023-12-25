// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Text.Json.Serialization;

namespace SimpleAPI.Domain.Base;

public abstract class QueryResult<T>
{
    public IReadOnlyList<T> Items { get; protected set; }
    public int Rows { get; protected set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int TotalRows { get; protected set; }

    protected QueryResult(IReadOnlyList<T> items, int totalRows = 0)
    {
        Items     = items;
        Rows      = items.Count;
        TotalRows = totalRows;
    }
}
