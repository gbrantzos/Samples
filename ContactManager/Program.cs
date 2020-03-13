using System;
using System.IO;
using System.Linq;
using Autofac.Extensions.DependencyInjection;
using ContactManager.API;
using FluentMigrator;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;

namespace ContactManager
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            ILogger currentLogger = null;
            try
            {
                currentLogger = GetNLogProvider().CreateLogger(typeof(Program).FullName);

                var serviceProvider = new ServiceCollection()
                    .AddFluentMigratorCore()
                    .ConfigureRunner(r =>
                    {
                        r.AddSqlServer();
                        r.WithGlobalConnectionString("Server=(local);Database=DDDSample;Trusted_Connection=True;MultipleActiveResultSets=true");
                        r.ScanIn(typeof(Program).Assembly);
                    })
                    .AddLogging(l => l.AddNLog())
                    .BuildServiceProvider(false);
                using(var scope = serviceProvider.CreateScope())
                {
                    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                    if (runner.HasMigrationsToApplyUp())
                    {
                        var context   = scope.ServiceProvider.GetRequiredService<IMigrationContext>();
                        var generator = scope.ServiceProvider.GetRequiredService<IMigrationGenerator>();

                        var info      = runner.MigrationLoader.LoadMigrations();
                        var migration = info[20200312001].Migration;

                        migration.GetUpExpressions(context);
                        var exp = context.Expressions.FirstOrDefault();

                        string script = generator.Generate((dynamic)exp);
                        currentLogger.LogWarning(script);
                        // runner.MigrateUp();
                    }
                };

                currentLogger.LogInformation("Contact Manager starting...");
                CreateHostBuilder(args)
                    .Build()
                    .Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Contact Manager failed to start!");
                Console.WriteLine(ex.ToString());

                currentLogger.LogError(ex, "Contact Manager failed to start!");
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads
                // before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var thisAssembly = typeof(Program).Assembly;

            return Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureServices((context, services) =>
                {
                    services.AddMediatR(thisAssembly);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // Setup Web stuff
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddNLog();
                });
        }

        private static ILoggerProvider GetNLogProvider()
        {
            var programPath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            var configFile = Path.Combine(programPath, "nlog.config");
            NLogBuilder.ConfigureNLog(configFile);

            return new NLogLoggerProvider();
        }
    }
}
