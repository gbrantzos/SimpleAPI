using SimpleAPI.Core;
using SimpleAPI.Core.Guards;

namespace SimpleAPI.Web.Setup.Context;

public class RequestContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RequestContextProvider _contextProvider;

    public RequestContextMiddleware(RequestDelegate next, RequestContextProvider contextProvider)
    {
        _next            = next.ThrowIfNull();
        _contextProvider = contextProvider.ThrowIfNull();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Ensure.NotNull(context);
        
        _contextProvider.CurrentContext = RequestContext.Create();
        context.Response.Headers.Append("X-Request-StartedOn", _contextProvider.CurrentContext.StartedOn.ToLongDateString());
        await _next(context);
        _contextProvider.CurrentContext = null;
    }
}
