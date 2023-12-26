// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Text.Json.Serialization;

namespace SimpleAPI.Domain.Base;

public abstract class QueryResult<T>
{
    public IReadOnlyList<T> Rows { get; protected set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int TotalRowCount { get; protected set; }

    protected QueryResult(IReadOnlyList<T> rows, int totalRows = 0)
    {
        Rows          = rows;
        TotalRowCount = totalRows;
    }
}
