namespace DDD.Domain.Core;

public interface IRepository<TEntity, in TID>
    where TEntity : Entity<TID>
    where TID : EntityID, new()
{
    /// <summary>
    /// Get <typeparamref name="TEntity"/> with given <typeparamref name="TID"/>.
    /// Throws <see cref="EntityNotFoundException"/> if no entity exists with given key.
    /// </summary>
    /// <param name="entityID">The ID of the entity to get</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The Item with given ID</returns>
    public Task<TEntity> GetByID(TID entityID, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Add a new <typeparamref name="TEntity"/> on repository.
    /// </summary>
    /// <param name="entity">The entity to add</param>
    /// <returns></returns>
    void Add(TEntity entity);

    /// <summary>
    /// Delete an existing <typeparamref name="TEntity"/>.
    /// </summary>
    /// <param name="entity">The entity to delete</param>
    /// <returns></returns>
    void Delete(TEntity entity);
}

public class EntityNotFoundException : Exception
{
    public Type EntityType { get; private init; }
    public EntityID ID { get; private init; }

    public EntityNotFoundException(Type entityType, EntityID id)
        : base($"Could not find entity {entityType.Name} with {id}")
    {
        EntityType = entityType ?? throw new ArgumentException(nameof(EntityType));
        ID = id ?? throw new ArgumentException(nameof(ID));
    }
}