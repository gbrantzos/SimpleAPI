namespace SimpleAPI.Application.Core;

public enum ErrorKind
{
    Generic,
    NotFound,
    ValidationFailed
}

public sealed class Error
{
    public ErrorKind Kind { get; private set; }
    public string Message { get; private set; }
    public Dictionary<string, object?>? Details { get; private set; }

    private Error(ErrorKind kind, string message, IDictionary<string, object?>? details = null)
    {
        Kind    = kind;
        Message = message;
        if (details is not null)
        {
            Details = new Dictionary<string, object>(details);
        }
    }

    public static Error Create(string message)
        => new Error(ErrorKind.Generic, message);

    public static Error Create(ErrorKind kind, string message)
        => new Error(kind, message);

    public static Error Create(ErrorKind kind, string message, IDictionary<string, object?> details)
        => new Error(kind, message, details);
}
