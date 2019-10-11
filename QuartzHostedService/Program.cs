using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

#if DEBUG
using System.Diagnostics;
using System.Runtime.InteropServices;
#endif

namespace QuartzHostedService
{
    public static class Program
    {
        #if DEBUG
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int cmdShow);
        private const int HIDE     = 0;
        private const int MAXIMIZE = 3;
        private const int MINIMIZE = 6;
        private const int RESTORE  = 9;
        #endif

        public static void Main(string[] args)
        {
            #if DEBUG
            Process p = Process.GetCurrentProcess();
            ShowWindow(p.MainWindowHandle, MAXIMIZE);
            #endif

            var logger = GetLogger();
            try
            {
                logger.Info("Starting application...");
                CreateHostBuilder(args)
                    .Build()
                    .Run();
                logger.Info("Application shutdown.");
            }
            catch (Exception ex)
            {
                // Catch setup errors
                logger.Error(ex, "Stopped program because of exception!");
                throw;
            }
            finally
            {
                // Shutdown logger
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .ConfigureLogging(logging => logging.ClearProviders())
                .UseNLog();

        private static NLog.Logger GetLogger()
        {
            // Setup the logger first to catch all errors
            var binPath    = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            var configPath = Path.Combine(binPath, "nlog.config");

            return NLogBuilder.ConfigureNLog(configPath).GetCurrentClassLogger();
        }
    }
}
