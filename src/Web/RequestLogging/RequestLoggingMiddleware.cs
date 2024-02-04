using SimpleAPI.Core;

namespace SimpleAPI.Web.RequestLogging;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RequestLoggingOptions _options;
    private readonly IRequestHandler _handler;

    public RequestLoggingMiddleware(RequestDelegate next, RequestLoggingOptions options, IRequestHandler handler)
    {
        _next    = next.ThrowIfNull();
        _options = options.ThrowIfNull();
        _handler = handler.ThrowIfNull();
    }

    public async Task InvokeAsync(DefaultHttpContext context)
    {
        await _next(context);
        _handler.Handle();
    }
}
