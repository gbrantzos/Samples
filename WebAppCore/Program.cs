using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace WebAppCore
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddYamlFile("SasConnection.yaml", optional: false, reloadOnChange: true);
                })
                .UseSerilog((ctx, config) => { config.ReadFrom.Configuration(ctx.Configuration); });
    }
}
