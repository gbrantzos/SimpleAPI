using System.Reflection;
using SimpleAPI.Web.Endpoints;
using SimpleAPI.Web.HostSetup.Environment;

namespace SimpleAPI.Web.HostSetup;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder,
        IConfigurationRoot configuration)
    {
        builder.MapCommonEndpoints(configuration);
        builder.MapFeatureEndpoints();
        
        return builder;
    }

    private static IEndpointRouteBuilder MapCommonEndpoints(this IEndpointRouteBuilder routeBuilder,
        IConfigurationRoot configuration)
    {
        var environment = routeBuilder.ServiceProvider.GetRequiredService<IEnvironment>();
        
        // Info on home page
        routeBuilder.MapGet("/", () => environment.GetLogo())
            .ExcludeFromDescription();

        // Check configuration
        routeBuilder.MapGet("/configuration", () => configuration.GetDebugView())
            .ExcludeFromDescription();

        // Favicon
        routeBuilder.MapGet("/favicon.ico",
            async context =>
            {
                var favIcon = environment.MainAssembly.GetManifestResourceStream("Web.Web.Logo.png") ??
                    throw new InvalidOperationException("Could not read favicon.ico!");
                var data = new byte[favIcon.Length];
                _ = await favIcon.ReadAsync(data.AsMemory(0, data.Length));
                context.Response.Headers.Add("Content-Type", "image/png");
                await context.Response.Body.WriteAsync(data);
            });

        return routeBuilder;
    }

    private static IEndpointRouteBuilder MapFeatureEndpoints(this IEndpointRouteBuilder builder, params Assembly[] assemblies)
    {
        var toScan = assemblies.Length == 0
            ? new[] { Assembly.GetExecutingAssembly() }
            : assemblies;
        var mappers = GetMappers(toScan);

        foreach (var mapper in mappers)
        {
            mapper.MapEndpoints(builder);
        }

        return builder;
    }

    private static IEnumerable<IEndpointMapper> GetMappers(params Assembly[] assemblies)
        => assemblies
            .SelectMany(a => a.GetTypes())
            .Where(p => p.IsClass && p.IsAssignableTo(typeof(IEndpointMapper)))
            .Select(Activator.CreateInstance)
            .Cast<IEndpointMapper>();
}
