using DDD.Domain.Core;
using DDD.Domain.Customers;
using Microsoft.EntityFrameworkCore;

namespace DDD.Persistence;

public class Context : DbContext
{
    // Audit Columns
    public const string CreatedAt = "CreatedAt";
    public const string ModifiedAt = "ModifiedAt";

    public DbSet<Customer> Customer => Set<Customer>();

    public Context(DbContextOptions<Context> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var newOrAdded = ChangeTracker
            .Entries()
            .Where(e => (e.State is EntityState.Added or EntityState.Modified) && EntryIsEntityID(e.Entity));

        foreach (var entry in newOrAdded)
        {
            // Change created and updated timestamps
            if (entry.State == EntityState.Added)
                entry.Property(CreatedAt).CurrentValue = DateTime.Now;
            entry.Property(ModifiedAt).CurrentValue = DateTime.Now;
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    private static bool EntryIsEntityID(object entry)
    {
        var entryType = entry.GetType();
        return entryType.BaseType != null
               && (entryType.BaseType?.GetGenericTypeDefinition().IsAssignableFrom(typeof(Entity<>)) ?? false)
               && (entryType.BaseType?.GetGenericArguments()
                       .FirstOrDefault()
                       ?.BaseType?.IsAssignableFrom(typeof(EntityID))
                   ?? false);
    }
}