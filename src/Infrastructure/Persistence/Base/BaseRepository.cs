using Microsoft.EntityFrameworkCore;
using SimpleAPI.Core;
using SimpleAPI.Domain.Base;

namespace SimpleAPI.Infrastructure.Persistence.Base;

public abstract class BaseRepository<TEntity, TEntityID> : IRepository<TEntity, TEntityID>
    where TEntity : Entity<TEntityID>
    where TEntityID : struct, IEntityID, IEquatable<TEntityID>
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

    public Task<int> CountAsync(Specification<TEntity> specification, CancellationToken cancellationToken = default) 
        => Context
            .Set<TEntity>()
            .Where(specification.Expression)
            .TagWith($"{RepositoryName} :: {nameof(CountAsync)}")
            .AsNoTracking()
            .CountAsync(cancellationToken);

    public async Task<IReadOnlyList<TEntity>> FindAsync(SearchCriteria<TEntity> criteria, 
        CancellationToken cancellationToken = default)
    {
        var dbSet = DbSetWithDetails(Context.Set<TEntity>(), criteria.Include);
        var query = dbSet.Where(criteria.Specification.Expression);
        var sortedQuery = AddSorting(query, criteria.Sorting);

        var results = await sortedQuery
            .TagWith($"{RepositoryName} :: {nameof(FindAsync)}")
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return results;
    }

    private static IQueryable<TEntity> DbSetWithDetails(IQueryable<TEntity> query, IEnumerable<string> details)
        => details.Aggregate(query, (current, detail) => current.Include(detail));

    private static IOrderedQueryable<TEntity> AddSorting(IQueryable<TEntity> query, IEnumerable<Sorting<TEntity>> sorting)
    {
        var sortingList = sorting.ToList();
        if (sortingList.Count == 0)
            return query.OrderBy(e => e.ID);

        IOrderedQueryable<TEntity>? sortedQuery = null;
        foreach (var sort in sortingList)
        {
            if (sort.OrderBy is null) continue;

            sortedQuery = sort.Direction switch
            {
                Sorting.SortDirection.Ascending => sortedQuery is null
                    ? query.OrderBy(sort.OrderBy)
                    : sortedQuery.ThenBy(sort.OrderBy),
                Sorting.SortDirection.Descending => sortedQuery is null
                    ? query.OrderByDescending(sort.OrderBy)
                    : sortedQuery.ThenByDescending(sort.OrderBy),
                _ => throw new ArgumentException($"Unsupported sorting direction {sort}")
            };
        }

        return sortedQuery!;
    }
}
