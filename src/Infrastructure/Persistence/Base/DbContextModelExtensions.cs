using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SimpleAPI.Infrastructure.Persistence.Base;

public static class DbContextModelExtensions
{
    public static ReferenceCollectionBuilder<TEntity, TRelatedEntity> HasRelatedTableIndexName<TEntity, TRelatedEntity>(
        this ReferenceCollectionBuilder<TEntity, TRelatedEntity> referenceCollectionBuilder, string name)
        where TEntity : class
        where TRelatedEntity : class
    {
        referenceCollectionBuilder.Metadata
            .Properties[0]
            .GetContainingIndexes()
            .FirstOrDefault()
            ?.SetDatabaseName(name);
        return referenceCollectionBuilder;
    }
}
