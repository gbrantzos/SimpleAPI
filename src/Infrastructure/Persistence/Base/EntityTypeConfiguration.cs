using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleAPI.Domain.Base;

namespace SimpleAPI.Infrastructure.Persistence.Base;

public abstract class EntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : Entity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        var entityName = typeof(TEntity).Name.ToLower(); // TODO This should be snake_case
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
    }
}
