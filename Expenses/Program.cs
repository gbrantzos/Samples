using System;
using System.Threading.Tasks;
using Expenses.Domain;
using Expenses.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Expenses
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            var connectionString = "Server=(local);Database=Expenses;Trusted_Connection=true;";
            var optionsBuilder = new DbContextOptionsBuilder<ExpensesDbContext>()
                .UseSqlServer(connectionString)
                .UseLoggerFactory(loggerFactory);
            optionsBuilder.EnableDetailedErrors(true);
            optionsBuilder.EnableSensitiveDataLogging(true);

            using var db = new ExpensesDbContext(optionsBuilder.Options);
            db.Database.EnsureCreated();

            if (await db.Buildings.CountAsync() == 0)
            {
                var address = new Address("23 Giannitson Str");
                var building = new Building("Giannitson 23", address);

                db.Add(building);
                await db.SaveChangesAsync();
            }
            else
            {
                var building = await db.Buildings.FirstOrDefaultAsync();
            }

            Console.WriteLine($"Buildings: {await db.Buildings.CountAsync()}");
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}
