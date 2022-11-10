using DDD.Domain.Core;
using Microsoft.EntityFrameworkCore;

namespace DDD.Persistence;

public abstract class GenericRepository<TEntity, TID> : IRepository<TEntity, TID>
    where TEntity : Entity<TID>
    where TID : EntityID, new()
{
    protected readonly Context Context;
    protected readonly DbSet<TEntity> DbSet;
    protected readonly string RepositoryName;

    protected GenericRepository(Context context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        
        DbSet = Context.Set<TEntity>();
        RepositoryName = GetType().Name;
    }

    /// <summary>
    /// Returns an array with navigation properties that should be included in GetByID results.
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerable<string> NavigationProperties => Array.Empty<string>();

    public virtual async Task<TEntity> GetByID(TID id, CancellationToken cancellationToken)
    {
        var queryable = NavigationProperties
            .Aggregate(DbSet.AsQueryable(), (query, navigationProperty) => query.Include(navigationProperty));
        
        var entity = await queryable
            .TagWith($"{RepositoryName} :: {nameof(GetByID)}")
            .FirstOrDefaultAsync(e => e.ID! == id, cancellationToken: cancellationToken);
        return entity ?? throw new EntityNotFoundException(typeof(TEntity), id);
    }

    public virtual void Add(TEntity entity) => DbSet.Add(entity);

    public virtual void Delete(TEntity entity) => DbSet.Remove(entity);
}