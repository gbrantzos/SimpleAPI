namespace SimpleAPI.Core.Base;

public sealed class Result<TData>
{
    private readonly TData? _data;
    private readonly Error? _error;

    public bool HasErrors { get; }

    private Result(TData? data, Error? error = default, bool hasErrors = false)
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

    #pragma warning disable CA2225
    public static implicit operator Result<TData>(TData data) => new(data);
    public static implicit operator Result<TData>(Error error) => new(default, error, true);
    public static implicit operator TData(Result<TData> result) => result.Data;
    #pragma warning restore CA2225
    
    public T Match<T>(Func<TData, T> dataFunc, Func<Error, T> errorFunc)
    {
        ArgumentNullException.ThrowIfNull(dataFunc);
        ArgumentNullException.ThrowIfNull(errorFunc);

        return HasErrors
            ? errorFunc(_error ?? throw new ArgumentException(nameof(_error)))
            : dataFunc(_data ?? throw new ArgumentException(nameof(_data)));
    }

    public void Match(Action<TData> dataAction, Action<Error> errorAction)
    {
        ArgumentNullException.ThrowIfNull(dataAction);
        ArgumentNullException.ThrowIfNull(errorAction);
        
        if (HasErrors)
            errorAction(_error ?? throw new ArgumentException(nameof(_error)));
        else
            dataAction(_data ?? throw new ArgumentException(nameof(_data)));
    }

    public TData Data =>
        HasErrors
            ? throw new InvalidOperationException("Result has errors.")
            : _data!;

    public Error Error =>
        HasErrors
            ? _error!
            : throw new InvalidOperationException("Result does not have errors.");
}

// TODO Result improvements
// Create methods
// - Map & MapError, which transforms Data
// - Bind & BindError, which returns a method that returns a new Result with Data
// - Tap, to invoke side effects
// - Ensure, to fail if a condition applies
// - Finally, to return a value and finish chain
// 
// Also create shortcuts for creation
// Consider having a Result without TData
