using Expenses.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

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

    /// <summary>
    /// Design time support
    /// </summary>
    public class ExpensesDbContextFactory : IDesignTimeDbContextFactory<ExpensesDbContext>
    {
        public ExpensesDbContext CreateDbContext(string[] args)
        {
            var connectionString = "Server=(local);Database=Expenses;Trusted_Connection=true;";

            var optionsBuilder = new DbContextOptionsBuilder<ExpensesDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ExpensesDbContext(optionsBuilder.Options);
        }
    }
}
