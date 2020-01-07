using Expenses.Domain;
using System.Threading.Tasks;

namespace Expenses.Infrastructure
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly ExpensesDbContext dbContext;

        public ExpenseRepository(ExpensesDbContext dbContext)
            => this.dbContext = dbContext;

        public async Task<Expense> GetById(long expenseId)
            => await dbContext.Expenses.FindAsync(expenseId);

        public Task Save(Expense expense)
        {
            dbContext.Expenses.Add(expense);
            return dbContext.SaveChangesAsync();
        }
    }
}
