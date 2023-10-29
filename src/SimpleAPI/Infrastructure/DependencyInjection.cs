using SimpleAPI.Application;
using SimpleAPI.Infrastructure.Setup;

namespace SimpleAPI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton(new ContextProvider<RequestContext>());
        services.AddSingleton<RequestContextEnricher>();
        
        return services;
    }
}
