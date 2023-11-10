using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleAPI.Application.Common;
using SimpleAPI.Domain.Features.Items;
using SimpleAPI.Infrastructure.Persistence;
using SimpleAPI.Infrastructure.Persistence.Repositories;

namespace SimpleAPI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration) 
        => services.AddPersistenceServices(configuration);

    private static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SimpleAPI");
        services.AddDbContext<SimpleAPIContext>(options => options
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors());

        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
