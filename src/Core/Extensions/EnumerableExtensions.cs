namespace SimpleAPI.Core;

public static class EnumerableExtensions
{
    // Details found on https://stackoverflow.com/a/21647841/3410871
    
    /// <summary>
    /// Convert an IEnumerable to IList. If source is already IList cast to avoid extra allocations.
    /// </summary>
    /// <param name="source">Source enumerable</param>
    /// <typeparam name="T">Type of source elements</typeparam>
    /// <returns></returns>
    public static IList<T> SmartToList<T>(this IEnumerable<T> source)
        =>  source as IList<T> ?? source.ToList();
}
