using System.Threading.Tasks;

namespace Expenses.Domain
{
    public interface IExpenseRepository
    {
        Task<Expense> GetById(long expenseId);

        Task Save(Expense expense);
    }
}
