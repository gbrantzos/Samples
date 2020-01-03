using System;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EfCore
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            //Add - Migration - Name Initialization - Project EfCore - StartupProject EfCore
            //Remove - Migration - Project EfCore
            //Update - Database - Project EfCore

            var connectionString = "Server=(local);Database=Sample;Trusted_Connection=true;";
            var optionsBuilder = new DbContextOptionsBuilder<SampleContext>();
            optionsBuilder.UseSqlServer(connectionString);

            using (var db = new SampleContext(optionsBuilder.Options))
            {
                var appliedMigrations = db.Database.GetAppliedMigrations();
                var pendingMigrations = db.Database.GetPendingMigrations();
                var lastMigration = appliedMigrations.LastOrDefault();

                if (pendingMigrations.Any())
                {
                    var migrator = db.GetService<IMigrator>();

                    Console.WriteLine($"There are pending migrations:");
                    foreach (var migration in pendingMigrations)
                    {
                        Console.WriteLine($"- {migration}");
                        Console.WriteLine("Script: ");

                        var script = migrator.GenerateScript(null, migration, false);
                        Console.WriteLine(script);
                    }
                    Console.WriteLine();
                    Console.WriteLine("Press any key to continue ...");
                    Console.ReadKey(true);
                }
            }
        }
    }
}
