using SimpleAPI.Web.ErrorMapping;
using SimpleAPI.Web.Setup.Context;
using SimpleAPI.Web.Setup.Environment;

namespace SimpleAPI.Web.Setup;

public static class DependencyInjection
{
    public static IServiceCollection AddHostServices(this IServiceCollection services, IEnvironment environment)
    {
        // Force returning BadRequest instead of throwing
        services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = false);

        services
            .AddEndpointsApiExplorer()
            .AddHttpContextAccessor()
            .AddProblemDetails(options => options.CustomizeProblemDetails = ErrorMapper.CustomizeProblemDetails)
            .AddSwaggerGen();

        services.AddSingleton(environment);
        services.AddSingleton<RequestContextProvider>();
        services.AddSingleton<RequestContextEnricher>();
        services.AddTransient<ErrorMapper>();

        return services;
    }
}
