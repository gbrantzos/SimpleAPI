using System.Reflection;

namespace SimpleAPI.Infrastructure.Endpoints;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder, params Assembly[] assemblies)
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
