using Microsoft.EntityFrameworkCore;
using SimpleAPI.Application.Common;
using SimpleAPI.Domain.Base;
using SimpleAPI.Domain.Features.Items;

namespace SimpleAPI.Infrastructure.Persistence;

public class SimpleAPIContext : DbContext
{
    private readonly ITimeProvider _timeProvider;

    // Common columns
    public const string ID = "id";
    public const string CreatedAt = "created_at";
    public const string ModifiedAt = "modified_at";
    public const string RowVersion = "row_version";

    public DbSet<Item> Items => Set<Item>();

    public SimpleAPIContext(DbContextOptions<SimpleAPIContext> options, ITimeProvider timeProvider) : base(options)
    {
        _timeProvider = timeProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SimpleAPIContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = new())
    {
        var newOrAdded = ChangeTracker
            .Entries()
            .Where(e => e is { Entity: Entity, State: EntityState.Added or EntityState.Modified });
        foreach (var entry in newOrAdded)
        {
            // Change created and updated timestamps
            if (entry.State == EntityState.Added)
                entry.Property(CreatedAt).CurrentValue = _timeProvider.GetNow();
            entry.Property(ModifiedAt).CurrentValue = _timeProvider.GetNow();
        }

        // Proper ROW_VERSION number for modified entries
        var allVersionedEntries = ChangeTracker.Entries<IVersioned>();
        foreach (var entry in allVersionedEntries)
        {
            // Increase row version
            if (entry.State == EntityState.Modified || entry.Navigations.Any(n => n.IsModified))
                entry.Entity.RowVersion++;

            // Proper initial ROW_VERSION value for new entries
            if (entry.Entity.RowVersion == 0) entry.Entity.RowVersion = 1;
        }
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public string GetDbSchema() => this.Database.GenerateCreateScript();
}
