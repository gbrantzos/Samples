namespace DDD.Domain.Core;

public interface IUnitOfWork
{
    public Task SaveChanges(CancellationToken cancellationToken = default);
}