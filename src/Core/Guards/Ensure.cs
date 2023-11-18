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
    public static void NotNull<T>(T parameter,
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
    /// Ensure that given condition is true
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="condition"></param>
    /// <param name="conditionName"></param>
    /// <param name="message"></param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="InvalidOperationException"></exception>
    public static void Applies<T>(T obj, 
        Predicate<T> condition,
        [CallerArgumentExpression("condition")]
        string? conditionName = null,
        string? message = null) where T : class
    {
        if (condition(obj))
        {
            throw new InvalidOperationException($"{conditionName} should be true");
        }
    }
}
