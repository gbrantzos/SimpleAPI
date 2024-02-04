namespace SimpleAPI.Web.RequestLogging;

public delegate Task RequestLoggingHandler(); 
public class RequestLoggingOptions
{
    public virtual RequestLoggingHandler? RequestLoggingHandler { get; set; }
}
