using SimpleAPI.Web.ErrorMapping;
using SimpleAPI.Web.HostSetup.Context;
using SimpleAPI.Web.HostSetup.Environment;

namespace SimpleAPI.Web.HostSetup;

public static class DependencyInjection
{
    public static IServiceCollection AddHostServices(this IServiceCollection services, IEnvironment environment)
    {
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
