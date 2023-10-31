using SimpleAPI.Infrastructure.Setup;

namespace SimpleAPI.Infrastructure;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseRequestContext(this IApplicationBuilder builder)
        => builder.UseMiddleware<RequestContextMiddleware>();
}
