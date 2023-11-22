using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SimpleAPI.Core;
using SimpleAPI.Domain.Base;

namespace SimpleAPI.Infrastructure.Persistence.Base;

public abstract class EntityTypeConfiguration<TEntity, TEntityID> : IEntityTypeConfiguration<TEntity>
    where TEntity : Entity<TEntityID>
    where TEntityID : IEntityID
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        var entityType = typeof(TEntity);
        var entityName = entityType.Name.ToSnakeCase();
        builder.ToTable(entityName);

        builder.HasKey(e => e.ID).HasName($"pk_{entityName}");
        builder.Property(e => e.ID)
            .HasColumnName(SimpleAPIContext.ID)
            .ValueGeneratedOnAdd()
            .HasColumnOrder(-20);

        builder.Property(e => e.RowVersion)
            .IsConcurrencyToken()
            .HasColumnOrder(-19)
            .HasColumnName(SimpleAPIContext.RowVersion);

        builder.Property<DateTime>(SimpleAPIContext.CreatedAt).HasColumnOrder(-12);
        builder.Property<DateTime>(SimpleAPIContext.ModifiedAt).HasColumnOrder(-11);

        builder.Ignore(e => e.IsNew);

        var properties = entityType
            .GetProperties()
            .Select(p => p.Name)
            .Except(new string[] { "ID", "IsNew", nameof(Entity.RowVersion) });
        foreach (var property in properties)
        {
            builder.Property(property).HasColumnName(property.ToSnakeCase());
        }
    }
}
