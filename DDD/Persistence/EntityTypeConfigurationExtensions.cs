using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDD.Persistence;

public static class EntityTypeConfigurationExtensions
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