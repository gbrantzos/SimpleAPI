using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SimpleAPI.Application.Common;
using SimpleAPI.Domain.Base;
using SimpleAPI.Domain.Features.Items;
using SimpleAPI.Infrastructure.Persistence.Configuration;

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

        // Since we use shadow FKs we need to configure Item first
        // Probably we need to somehow define this somewhere :(
        modelBuilder.ApplyConfiguration(new ItemEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ItemAlternativeCodesConfiguration());
        modelBuilder.ApplyConfiguration(new TagEntityConfiguration());
        
        // If we cannot solve the previous problem we cannot use the following
        // modelBuilder.ApplyConfigurationsFromAssembly(typeof(SimpleAPIContext).Assembly);
        
        // Normalize column ordering
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var propertyInfo in entityType.GetProperties())
            {
                if (propertyInfo.IsShadowProperty()) continue;
                var annotation = propertyInfo.GetAnnotations();
                var columnOrder = annotation.FirstOrDefault(a => a.Name == RelationalAnnotationNames.ColumnOrder);
                if (columnOrder is null)
                    propertyInfo.SetAnnotation(RelationalAnnotationNames.ColumnOrder, 30);
            }
        }
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = new())
    {
        HandleAuditable();
        HandleVersioned();

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        HandleAuditable();
        HandleVersioned();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    private void HandleVersioned()
    {
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
    }

    private void HandleAuditable()
    {
        var newOrModified = ChangeTracker
            .Entries<IAuditable>()
            .Where(e => e is { Entity: Entity, State: EntityState.Added or EntityState.Modified });
        foreach (var entry in newOrModified)
        {
            // Change created and updated timestamps
            if (entry.State == EntityState.Added)
                entry.Property(CreatedAt).CurrentValue = _timeProvider.GetNow();
            entry.Property(ModifiedAt).CurrentValue = _timeProvider.GetNow();
        }
    }

    public string GetDbSchema() => this.Database.GenerateCreateScript();
}
