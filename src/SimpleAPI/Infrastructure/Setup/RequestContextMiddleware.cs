namespace SimpleAPI.Infrastructure.Setup;

public class RequestContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RequestContextProvider _contextProvider;

    public RequestContextMiddleware(RequestDelegate next,RequestContextProvider contextProvider)
    {
        _next            = next;
        _contextProvider = contextProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _contextProvider.CurrentContext = new RequestContext();
        await _next(context);
    }
}

public static class RequestContextMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestContext(this IApplicationBuilder builder)
        => builder.UseMiddleware<RequestContextMiddleware>();
}
