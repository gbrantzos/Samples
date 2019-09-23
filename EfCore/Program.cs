using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EfCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EfCore
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var connectionString = "Server=(local);Database=Expenses;Trusted_Connection=True";

            var services = new ServiceCollection();
            services.AddSingleton<IHiLoGenerator>(c => new SqlServerHiLoGenerator(connectionString, 5));

            var serviceProvider = services.BuildServiceProvider();
            var optionBuilder = new DbContextOptionsBuilder<SampleDbContext>()
                .UseSqlServer(connectionString)
                .UseApplicationServiceProvider(serviceProvider);
            using (var db = new SampleDbContext(optionBuilder.Options))
            {
                var q1 = db.Expenses.Where(e => e.ExpenseID.ID > 20);

                Expression<Func<Expense, bool>> e1 = (e) => e.Amount.Amount > 12;
                Expression<Func<Expense, bool>> e2 = (e) => e.Amount.Amount < 2;

                var exp = PredicateBuilder.Or(e1, e2);

                var final = q1.Where(exp).ToList();

                var expenses = await db.Expenses.ToListAsync();
                foreach (var e in expenses)
                {
                    Console.WriteLine($"ID: {e.ExpenseID} - {e.Description}");
                }


                for (int i = 0; i < 20; i++)
                {
                    var ex = new Expense($"Dummy - {i}", false, DateTime.UtcNow, Money.InEuro(i), ExpenseCategory.Shared);
                    db.Add(ex);
                }
                db.SaveChanges();
            }

            using (var db = new SampleDbContext(optionBuilder.Options))
            {
                for (int i = 0; i < 20; i++)
                {
                    var ex = new Expense($"Dummy, extra - {i}", false, DateTime.UtcNow, Money.InEuro(i * 1.013m), ExpenseCategory.Shared);
                    db.Add(ex);
                }
                db.SaveChanges();
            }
            Console.ReadLine();
        }
    }
}
