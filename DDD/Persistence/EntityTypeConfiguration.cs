using System.Linq.Expressions;
using DDD.Domain.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DDD.Persistence;

public abstract class EntityTypeConfiguration<TEntity, TID> : IEntityTypeConfiguration<TEntity>
    where TEntity : Entity<TID>
    where TID : EntityID, new()
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        ConfigureEntityID(builder, p => p.ID);
        ConfigureRowVersion(builder, p => p.Version);
        ConfigureAuditFields(builder);
    }

    private static void ConfigureEntityID(EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, object?>> keyExpression)
    {
        builder.HasKey(keyExpression)
            .HasName($"PK_{typeof(TEntity).Name}");
        builder.Property(keyExpression)
            .HasConversion<EntityIDConverter<TID>>()
            .ValueGeneratedOnAdd()
            .HasColumnOrder(-49);
    }

    private static void ConfigureRowVersion<TProperty>(EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, TProperty>> propertyExpression)
    {
        builder.Property(propertyExpression)
            .IsRowVersion()
            .HasColumnOrder(-45);
    }

    private static void ConfigureAuditFields(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property<DateTime>(Context.CreatedAt).HasColumnOrder(-39);
        builder.Property<DateTime>(Context.ModifiedAt).HasColumnOrder(-38);
    }
}

public class EntityIDConverter<TEntityID> : ValueConverter<TEntityID, int> where TEntityID : EntityID, new()
{
    public EntityIDConverter() : base(v => v.Value, v => new TEntityID {Value = v})
    {
    }
}