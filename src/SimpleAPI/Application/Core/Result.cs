namespace SimpleAPI.Application.Core;

public sealed class Result<TData, TError>
{
    private readonly TData? _data;
    private readonly TError? _error;

    public bool HasErrors { get; }

    private Result(TData? data, TError? error = default, bool hasErrors = false)
    {
        if (hasErrors)
        {
            // When we do have an Error, make sure it is not be null
            ArgumentNullException.ThrowIfNull(error);
        }
        else
        {
            // When we don't have an Error, we should have data
            ArgumentNullException.ThrowIfNull(data);
        }

        _data     = data;
        _error    = error;
        HasErrors = hasErrors;
    }

    public static implicit operator Result<TData, TError>(TData data) => new(data);
    public static implicit operator Result<TData, TError>(TError error) => new(default, error, true);

    public T Match<T>(Func<TData, T> dataFunc, Func<TError, T> errorFunc)
    {
        ArgumentNullException.ThrowIfNull(dataFunc);
        ArgumentNullException.ThrowIfNull(errorFunc);

        return HasErrors
            ? errorFunc(_error ?? throw new ArgumentException(nameof(_error)))
            : dataFunc(_data ?? throw new ArgumentException(nameof(_data)));
    }

    public TData Data =>
        HasErrors
            ? throw new InvalidOperationException("Result has errors.")
            : _data ?? throw new ArgumentNullException(nameof(_data));

    public TError Error =>
        HasErrors
            ? _error ?? throw new ArgumentNullException(nameof(_error))
            : throw new InvalidOperationException("Result does not have errors.");
}
