using Microsoft.EntityFrameworkCore;
using SimpleAPI.Application.Common;
using SimpleAPI.Domain.Base;
using SimpleAPI.Domain.Features.Items;
using SimpleAPI.Infrastructure.Persistence.Configuration;

namespace SimpleAPI.Infrastructure.Persistence;

public class SimpleAPIContext : DbContext
{
    private readonly TimeProvider _timeProvider;

    // Common columns
    public const string ID = "id";
    public const string CreatedAt = "created_at";
    public const string ModifiedAt = "modified_at";
    public const string RowVersion = "row_version";
    public const string IsDeleted = "is_deleted";
    public const string DeletedAt = "deleted_at";

    public DbSet<Item> Items => Set<Item>();

    public SimpleAPIContext(DbContextOptions<SimpleAPIContext> options, TimeProvider timeProvider) : base(options)
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
        modelBuilder.ApplyConfiguration(new ItemFeatureEntityConfiguration());
        modelBuilder.ApplyConfiguration(new FeatureEntityConfiguration());

        // If we cannot solve the previous problem we cannot use the following
        // modelBuilder.ApplyConfigurationsFromAssembly(typeof(SimpleAPIContext).Assembly);
    }
    
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = new())
    {
        HandleSoftDeleted();
        HandleAuditable();
        HandleVersioned();

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
    
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        HandleSoftDeleted();
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
                entry.Property(CreatedAt).CurrentValue = _timeProvider.GetLocalNow().DateTime;
            if (entry.State == EntityState.Modified)
            {
                if (entry.Entity is ISoftDelete && entry.Property<bool>(IsDeleted).CurrentValue)
                    continue;
                entry.Property(ModifiedAt).CurrentValue = _timeProvider.GetLocalNow().DateTime;
            }
        }
    }

    private void HandleSoftDeleted()
    {
        var softDeleted = ChangeTracker
            .Entries<ISoftDelete>()
            .Where(e => e.Property<bool>(IsDeleted).CurrentValue);
        foreach (var entry in softDeleted)
        {
            entry.Property(DeletedAt).CurrentValue = _timeProvider.GetLocalNow().DateTime;
        }
    }

    public string GetDbSchema() => this.Database.GenerateCreateScript();
}
