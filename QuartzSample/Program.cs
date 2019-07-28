using System;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;

namespace QuartzSample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder()
               .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var configuration = configBuilder.Build();

            var services = new ServiceCollection();
            services.AddScoped<DemoJob>();
            services.AddScoped<IDemoService, DemoService>();
            services.AddOptions();
            services.Configure<DemoServiceConfig>(configuration.GetSection("DemoService"));

            var serviceProvider = services.BuildServiceProvider();

            var props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary"}
            };
            var factory = new StdSchedulerFactory(props);
            var scheduler = await factory.GetScheduler();
            scheduler.JobFactory = new JobFactory(serviceProvider);

            var job = JobBuilder
                .Create<DemoJob>()
                .WithIdentity("DemoJob", "QuartzSample")
                .Build();
            var trigger = TriggerBuilder
                .Create()
                .WithIdentity("DemoJob_Trigger", "QuartzSample")
                .StartNow()
                .WithSimpleSchedule(s => s.WithIntervalInSeconds(7).RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);
            await scheduler.Start();
            
            Console.ReadKey();
            await scheduler.Shutdown();
        }
    }
}
