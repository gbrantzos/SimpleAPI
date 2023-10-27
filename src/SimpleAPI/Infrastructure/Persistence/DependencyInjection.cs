using SimpleAPI.Domain;
using SimpleAPI.Domain.Items;
using SimpleAPI.Infrastructure.Persistence.Repositories;

namespace SimpleAPI.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
    {
        services.AddDbContext<SimpleAPIContext>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
