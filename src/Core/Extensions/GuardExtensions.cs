using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SimpleAPI.Core;

public static class GuardExtensions
{
    /// <summary>
    /// Handling the null check and throw a ArgumentNullException if null
    /// </summary>
    /// <typeparam name="T">The type of the parameter</typeparam>
    /// <param name="parameter">the actual parameter</param>
    /// <param name="parameterName">the name of the parameter</param>
    /// <param name="message">an optional message instead of the default</param>
    public static T ThrowIfNull<T>([NotNull] this T parameter, 
        [CallerArgumentExpression("parameter")] string? parameterName = null, 
        string? message = null) where T : class
    {
        if (parameter == null)
        {
            throw new ArgumentNullException(parameterName, message ?? $"Parameter '{parameterName}' is null!");
        }
        return parameter;
    }

    /// <summary>
    /// Handling string empty check and throw a ArgumentNullException if null
    /// </summary>
    /// <param name="parameter">the actual string parameter</param>
    /// <param name="parameterName">the name of the parameter</param>
    /// <param name="message">an optional message instead of the default</param>
    public static string ThrowIfEmpty(this string parameter, 
        [CallerArgumentExpression("parameter")] string? parameterName = null, 
        string? message = null)
    {
        if (string.IsNullOrEmpty(parameter))
        {
            throw new ArgumentNullException(parameterName, message ?? $"Parameter '{parameterName}' is null or empty!");
        }
        return parameter;
    }
}
