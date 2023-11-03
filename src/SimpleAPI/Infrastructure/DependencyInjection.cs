using FluentValidation;
using SimpleAPI.Domain.Core;
using SimpleAPI.Domain.Features.Items;
using SimpleAPI.Infrastructure.Behavior;
using SimpleAPI.Infrastructure.Persistence;
using SimpleAPI.Infrastructure.Persistence.Repositories;
using SimpleAPI.Infrastructure.Setup;

namespace SimpleAPI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<RequestContextProvider>();
        services.AddSingleton<RequestContextEnricher>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<ISimpleAPI>();
            cfg.AddOpenBehavior(typeof(CatchAllBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssemblyContaining<ISimpleAPI>();
        
        return services;
    }

    public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
    {
        services.AddDbContext<SimpleAPIContext>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
