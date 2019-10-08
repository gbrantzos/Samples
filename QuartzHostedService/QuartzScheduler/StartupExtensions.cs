using System;
using System.Collections.Specialized;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace QuartzHostedService.QuartzScheduler
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddQuartzHostedService(this IServiceCollection services,
            NameValueCollection quartzProperties = null)
        {
            // Add Quartz services
            var defaults = new NameValueCollection
            {
                { "quartz.scheduler.instanceName", "Quartz Scheduler" },
                { "quartz.threadPool.type", "Quartz.Simpl.SimpleThreadPool, Quartz" },
                { "quartz.threadPool.threadCount", "4" },
                { "quartz.jobStore.misfireThreshold", "60000" },
                { "quartz.serializer.type", "binary"}
            };
            if (quartzProperties == null)
                quartzProperties = defaults;
            {
                foreach (string key in defaults)
                {
                    if (String.IsNullOrEmpty(quartzProperties.Get(key)))
                        quartzProperties.Add(key, defaults.Get(key));
                }
            }
            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddSingleton<ISchedulerFactory>(new StdSchedulerFactory(quartzProperties));

            // Hosted Service
            services.AddHostedService<QuartzSchedulerService>();

            // Jobs scheduler
            services.AddSingleton<IJobScheduler, QuartzSchedulerService>();

            return services;
        }
    }
}
