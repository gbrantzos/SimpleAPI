using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace SimpleAPI.Infrastructure.Persistence.Base;

public static class QueryableExtensions
{
    public static IQueryable<T> TagWithDebugInfo<T>(this IQueryable<T> query,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        var debugInfo = $"Caller: {memberName}, file {filePath} - line {lineNumber}";
        return query.TagWith(debugInfo);
    }
}