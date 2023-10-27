using SimpleAPI.Infrastructure.Endpoints.Item;

namespace SimpleAPI.Infrastructure.Endpoints;

public static class EndpointRouteBuilder
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder.MapItemEndpoints();
    }
    
    private static IEndpointRouteBuilder MapItemEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/items/{id:int}", GetItemEndpoint.Handle);
        builder.MapPost("/items", SaveItemEndpoint.Handle);
        builder.MapDelete("/items/{id:int}", DeleteItemEndpoint.Handle);
        
        return builder;
    }
}
