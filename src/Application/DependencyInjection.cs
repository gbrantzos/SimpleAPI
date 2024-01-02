using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SimpleAPI.Application.Common;
using SimpleAPI.Application.Common.Behavior;

namespace SimpleAPI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var thisAssembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(thisAssembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(MetricsBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(thisAssembly);
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
