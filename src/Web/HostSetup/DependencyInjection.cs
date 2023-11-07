using SimpleAPI.Web.ErrorMapping;
using SimpleAPI.Web.HostSetup.Context;
using SimpleAPI.Web.HostSetup.Environment;

namespace SimpleAPI.Web.HostSetup;

public static class DependencyInjection
{
    public static IServiceCollection AddSystemServices(this IServiceCollection services)
    {
        services
            .AddEndpointsApiExplorer()
            .AddHttpContextAccessor()
            .AddProblemDetails(options => options.CustomizeProblemDetails = ErrorMapper.CustomizeProblemDetails)
            .AddSwaggerGen();

        return services;
    }

    

    public static IServiceCollection AddHostServices(this IServiceCollection services, IEnvironment environment)
    {
        
        services.AddSingleton(environment);
        services.AddSingleton<RequestContextProvider>();
        services.AddSingleton<RequestContextEnricher>();
        services.AddTransient<ErrorMapper>();

        return services;
    }
    
    
}
