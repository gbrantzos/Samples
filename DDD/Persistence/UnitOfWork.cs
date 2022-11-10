using DDD.Domain.Core;

namespace DDD.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly Context _context;

    public UnitOfWork(Context context) => _context = context;

    public Task SaveChanges(CancellationToken cancellationToken) => _context.SaveChangesAsync(cancellationToken);
}