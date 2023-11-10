namespace SimpleAPI.Web.Setup.Context;

public class RequestContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RequestContextProvider _contextProvider;

    public RequestContextMiddleware(RequestDelegate next, RequestContextProvider contextProvider)
    {
        _next            = next;
        _contextProvider = contextProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _contextProvider.CurrentContext = RequestContext.Create();
        context.Response.Headers.Add("X-Request-StartedOn", _contextProvider.CurrentContext.StartedOn.ToLongDateString());
        await _next(context);
        _contextProvider.CurrentContext = null;
    }
}
