using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleAPI.Core;
using SimpleAPI.Domain.Base;
// ReSharper disable StaticMemberInGenericType

namespace SimpleAPI.Infrastructure.Persistence.Base;

public abstract class EntityTypeConfiguration<TEntity, TEntityID> : IEntityTypeConfiguration<TEntity>
    where TEntity : Entity<TEntityID>
    where TEntityID : IEntityID
{
    private readonly Type _idConversion;
    private readonly IEnumerable<string> _skipProperties;
    private static readonly string[] BasicProperties = new string[] { "ID", "IsNew", "RowVersion" };

    protected EntityTypeConfiguration(Type idConversion, IEnumerable<string>? skipProperties = null)
    {
        _idConversion   = idConversion.ThrowIfNull();
        _skipProperties = skipProperties ?? Array.Empty<string>();
    }

    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        var entityType = typeof(TEntity);
        var entityName = entityType.Name.ToSnakeCase();
        builder.ToTable(entityName);

        builder.HasKey(e => e.ID).HasName($"pk_{entityName}");
        builder.Property(e => e.ID)
            .HasColumnName(SimpleAPIContext.ID)
            .HasConversion(_idConversion)
            .ValueGeneratedOnAdd()
            .HasColumnOrder(-20);

        if (typeof(TEntity).GetInterfaces().Contains(typeof(IVersioned)))
        {
            builder.Property(e => ((IVersioned)e).RowVersion)
                .IsConcurrencyToken()
                .HasColumnOrder(-19)
                .HasColumnName(SimpleAPIContext.RowVersion);
        }

        builder.Property<DateTime>(SimpleAPIContext.CreatedAt).HasColumnOrder(-12);
        builder.Property<DateTime>(SimpleAPIContext.ModifiedAt).HasColumnOrder(-11);

        builder.Ignore(e => e.IsNew);

        var skipProperties = BasicProperties.Concat(_skipProperties);
        var properties = entityType
            .GetProperties()
            .Select(p => p.Name)
            .Except(skipProperties);
        foreach (var property in properties)
        {
            builder.Property(property).HasColumnName(property.ToSnakeCase());
        }
    }
}
