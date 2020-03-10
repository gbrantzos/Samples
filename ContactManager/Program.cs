using System;
using System.IO;
using ContactManager.API;
using MediatR;
using Microsoft.AspNetCore.Hosting;
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
