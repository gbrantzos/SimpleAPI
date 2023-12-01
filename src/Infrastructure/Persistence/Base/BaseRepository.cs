using Microsoft.EntityFrameworkCore;
using SimpleAPI.Core;
using SimpleAPI.Domain.Base;

namespace SimpleAPI.Infrastructure.Persistence.Base;

public abstract class BaseRepository<TEntity, TEntityID> : IRepository<TEntity, TEntityID>
    where TEntity : Entity<TEntityID>
    where TEntityID : IEntityID, IEquatable<TEntityID>
{
    protected SimpleAPIContext Context { get; }
    protected virtual IReadOnlyCollection<string> DefaultDetails => Array.Empty<string>();
    protected string RepositoryName => this.GetType().Name;

    protected BaseRepository(SimpleAPIContext context)
        => Context = context.ThrowIfNull();

    public async Task<TEntity?> GetByIDAsync(TEntityID id, CancellationToken cancellationToken = default)
    {
        // We should perform split queries unless we have a valid reason not to
        // https://learn.microsoft.com/en-us/ef/core/querying/single-split-queries

        var set = DbSetWithDetails(Context.Set<TEntity>(), DefaultDetails);
        var result = await set
            .TagWith($"{RepositoryName} :: {nameof(GetByIDAsync)}")
            .AsSplitQuery()
            .SingleOrDefaultAsync(i => id.Equals(i.ID), cancellationToken);
        return result;
    }

    public void Add(TEntity entity) => Context.Set<TEntity>().Add(entity);

    public void Delete(TEntity entity)
    {
        if (entity is ISoftDelete)
        {
            Context.Entry(entity)
                .Property(SimpleAPIContext.IsDeleted)
                .CurrentValue = true;
            Context.Entry(entity).State = EntityState.Modified;
            return;
        }
        Context.Set<TEntity>().Remove(entity);
    }

    private static IQueryable<TEntity> DbSetWithDetails(IQueryable<TEntity> query, IEnumerable<string> details)
        => details.Aggregate(query, (current, detail) => current.Include(detail));
}
