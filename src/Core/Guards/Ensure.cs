using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SimpleAPI.Core.Guards;

public static class Ensure
{
    /// <summary>
    /// Ensure that given parameter is not null
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="parameterName"></param>
    /// <param name="message"></param>
    /// <typeparam name="T"></typeparam>
    public static void NotNull<T>([NotNull] T parameter,
        [CallerArgumentExpression("parameter")]
        string? parameterName = null,
        string? message = null) where T : class
        => parameter.ThrowIfNull(parameterName, message);

    /// <summary>
    /// Ensure given parameter (string) is not empty
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="parameterName"></param>
    /// <param name="message"></param>
    public static void NotEmpty(string parameter,
        [CallerArgumentExpression("parameter")]
        string? parameterName = null,
        string? message = null)
        => parameter.ThrowIfEmpty();

    /// <summary>
    /// Ensure given parameter (IEnumerable) is not empty
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="parameterName"></param>
    /// <param name="message"></param>
    public static void NotEmpty<T>(IEnumerable<T> parameter,
        [CallerArgumentExpression("parameter")]
        string? parameterName = null,
        string? message = null)
    {
        if (!parameter.Any())
        {
            throw new ArgumentException(message ?? $"{parameterName} cannot be empty", parameterName);
        }
    }

    /// <summary>
    /// Ensure that given condition is true
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="condition"></param>
    /// <param name="message"></param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="InvalidOperationException"></exception>
    public static void Applies<T>(T obj, 
        Predicate<T> condition,
        string message) where T : class
    {
        if (!condition(obj))
        {
            throw new ArgumentException(message);
        }
    }
}
