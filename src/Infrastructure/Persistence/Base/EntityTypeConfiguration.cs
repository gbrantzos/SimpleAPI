using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleAPI.Core;
using SimpleAPI.Domain.Base;

namespace SimpleAPI.Infrastructure.Persistence.Base;

public abstract class EntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : Entity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        var entityType = typeof(TEntity);
        var entityName = entityType.Name.ToSnakeCase();
        builder.ToTable(entityName);

        builder.HasKey(e => e.ID).HasName($"pk_{entityName}");
        builder.Property(e => e.ID)
            .HasColumnName(SimpleAPIContext.ID)
            .HasColumnOrder(-20);

        builder.Property(e => e.RowVersion)
            .IsConcurrencyToken()
            .HasColumnOrder(-19)
            .HasColumnName(SimpleAPIContext.RowVersion);

        builder.Property<DateTime>(SimpleAPIContext.CreatedAt).HasColumnOrder(-12);
        builder.Property<DateTime>(SimpleAPIContext.ModifiedAt).HasColumnOrder(-11);

        var properties = entityType
            .GetProperties()
            .Select(p => p.Name)
            .Except(new string[] { nameof(Entity.ID), nameof(Entity.RowVersion) });
        foreach (var property in properties)
        {
            builder.Property(property).HasColumnName(property.ToSnakeCase());
        }
    }
}
