using Expenses.Domain;
using Microsoft.EntityFrameworkCore;

namespace Expenses.Infrastructure
{
    public class ExpensesDbContext : DbContext
    {
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Building> Buildings { get; set; }

        public ExpensesDbContext(DbContextOptions<ExpensesDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ExpensesDbContext).Assembly);
        }
    }
}
