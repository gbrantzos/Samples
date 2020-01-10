using Expenses.Domain;
using Expenses.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Expenses
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var connectionString = "Server=(local);Database=Expenses;Trusted_Connection=True;MultipleActiveResultSets=true";
            var optionsBuilder = new DbContextOptionsBuilder<ExpensesDbContext>()
                .UseSqlServer(connectionString);

            var expense = new Expense(DateTime.Now, DateTime.Now, "Καθαριότητα", Money.Euro(58));
            //var expense = new Expense(new ExpenseId(), DateTime.Now, DateTime.Now, "Καθαριότητα", Money.Euro(58));

            using var db = new ExpensesDbContext(optionsBuilder.Options);
            db.Database.EnsureCreated();

            var repository = new ExpenseRepository(db);

            var id = 659082018365112320;
            var existing = await repository.GetById(id);
            // await repository.Save(expense);

            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}
