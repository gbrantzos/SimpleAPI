namespace SimpleAPI.Web.ErrorMapping;

public sealed class ExceptionObject
{
    public string Message { get; set; }
    public string ExceptionType { get; set; }
    public string? StackTrace { get; set; }
    public ExceptionObject? InnerException { get; set; }
    
    public ExceptionObject(Exception exception)
    {
        ExceptionType = exception.GetType().Name;
        Message       = exception.Message;
        StackTrace    = exception.StackTrace;
        if (exception.InnerException is { } innerException)
            InnerException = new ExceptionObject(innerException);
    }
}
