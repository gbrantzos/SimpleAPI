namespace SimpleAPI.Domain.Base;

public class SearchCriteria<T>
{
    public Specification<T> Specification { get; }
    public IEnumerable<string> Include { get; }
    public IEnumerable<Sorting<T>> OrderBy { get; }

    public SearchCriteria(Specification<T>? specification = null,
        IEnumerable<string>? include = null,
        IEnumerable<Sorting<T>>? orderBy = null)
    {
        Specification = specification ?? Specification<T>.True;
        Include       = include ?? Enumerable.Empty<string>();
        OrderBy       = orderBy ?? Enumerable.Empty<Sorting<T>>();
    }

    public SearchCriteria(Specification<T>? specification = null,
        IEnumerable<string>? include = null,
        Sorting<T>? orderBy = null) : this(
        specification,
        include,
        orderBy is null ? null : new[] { orderBy }) { }
}
