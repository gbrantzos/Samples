using EfCore.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCore
{
    public class SampleDbContext : DbContext
    {
        public DbSet<Expense> Expenses { get; set; }

        public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.ApplyConfigurationsFromAssembly(typeof(SampleDbContext).Assembly);
    }
}
