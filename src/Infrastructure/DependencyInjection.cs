using Microsoft.Extensions.DependencyInjection;
using SimpleAPI.Application.Common;
using SimpleAPI.Domain.Features.Items;
using SimpleAPI.Infrastructure.Persistence;
using SimpleAPI.Infrastructure.Persistence.Repositories;

namespace SimpleAPI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddPersistenceServices();

        return services;
    }

    private static IServiceCollection AddPersistenceServices(this IServiceCollection services)
    {
        services.AddDbContext<SimpleAPIContext>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
