using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleAPI.Core;
using SimpleAPI.Domain.Base;

// ReSharper disable StaticMemberInGenericType

namespace SimpleAPI.Infrastructure.Persistence.Base;

public abstract class EntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : Entity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        var entityType = typeof(TEntity);
        var entityName = entityType.Name.ToSnakeCase();
        builder.ToTable(entityName);

        if (typeof(TEntity).GetInterfaces().Contains(typeof(IVersioned)))
        {
            builder.Property(e => ((IVersioned)e).RowVersion)
                .IsConcurrencyToken()
                .HasColumnOrder(-21)
                .HasColumnName(SimpleAPIContext.RowVersion);
        }

        if (typeof(TEntity).GetInterfaces().Contains(typeof(IAuditable)))
        {
            builder.Property<DateTime>(SimpleAPIContext.CreatedAt).HasColumnOrder(101);
            builder.Property<DateTime?>(SimpleAPIContext.ModifiedAt).HasColumnOrder(103);
        }

        if (typeof(TEntity).GetInterfaces().Contains(typeof(ISoftDelete)))
        {
            builder.Property<bool>(SimpleAPIContext.IsDeleted).HasColumnOrder(105);
            builder.Property<DateTime?>(SimpleAPIContext.DeletedAt).HasColumnOrder(106);
            builder.HasQueryFilter(e => EF.Property<bool>(e, SimpleAPIContext.IsDeleted) == false);
        }
    }
}


public abstract class EntityTypeConfiguration<TEntity, TEntityID> : EntityTypeConfiguration<TEntity>
    where TEntity : Entity<TEntityID>
    where TEntityID : struct, IEntityID
{
    private readonly Type _idConversion;
    
    protected EntityTypeConfiguration(Type idConversion)
    {
        _idConversion   = idConversion.ThrowIfNull();
    }

    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        var entityType = typeof(TEntity);
        var entityName = entityType.Name.ToSnakeCase();

        base.Configure(builder);
        
        builder.HasKey(e => e.ID).HasName($"pk_{entityName}");
        builder.Property(e => e.ID)
            .HasColumnName(SimpleAPIContext.ID)
            .HasConversion(_idConversion)
            .ValueGeneratedOnAdd()
            .HasColumnOrder(-22);

        builder.Ignore(e => e.IsNew);        
    }
}
