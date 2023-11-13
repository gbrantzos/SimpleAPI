using Microsoft.EntityFrameworkCore;
using SimpleAPI.Domain.Base;

namespace SimpleAPI.IntegrationTests.Setup;

public static class DbContextExtensions
{
    public static void ExecuteAndRollback(this DbContext context, Action executable)
    {
        var strategy = context.Database.CreateExecutionStrategy();
        strategy.Execute(() =>
        {
            using var trans = context.Database.BeginTransaction();
            executable();
            trans.Rollback();
            context.ChangeTracker.Clear();
        });
    }

    public static async Task ExecuteAndRollbackAsync(this DbContext context, Func<Task> executable)
    {
        var strategy = context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var trans = await context.Database.BeginTransactionAsync();
            await executable();
            await trans.RollbackAsync();
            context.ChangeTracker.Clear();
        });
    }

    public static async Task<int> PersistEntityAsync<TEntity, TEntityID>(this DbContext context, TEntity entity) 
        where TEntity : Entity<TEntityID>
        where TEntityID : EntityID, new()
    {
        context.Add(entity);
        await context.SaveChangesAsync();
        return entity.ID?.Value ?? 0;
    }
}
