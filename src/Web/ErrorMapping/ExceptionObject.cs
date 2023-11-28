using System.Text.Json.Serialization;

namespace SimpleAPI.Web.ErrorMapping;

public sealed class ExceptionObject
{
    public string Message { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IEnumerable<string> InnerMessages { get; init; }

    public string ExceptionType { get; set; }
    public string? StackTrace { get; set; }
    public ExceptionObject? InnerException { get; set; }

    public ExceptionObject(Exception exception)
    {
        ExceptionType = exception.GetType().Name;
        Message       = exception.Message;
        InnerMessages = GetInnerExceptionMessages(exception);
        StackTrace    = exception.StackTrace;

        if (exception.InnerException is { } innerException)
            InnerException = new ExceptionObject(innerException);
    }

    private static IEnumerable<string> GetInnerExceptionMessages(Exception x)
    {
        var innerMessages = new List<string>();
        var inner = x.InnerException;
        while (inner is not null)
        {
            innerMessages.Add(inner.Message);
            inner = inner.InnerException;
        }
        return innerMessages;
    }
}
