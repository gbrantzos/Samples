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
        public static IServiceCollection AddScheduler(this IServiceCollection services,
            NameValueCollection quartzProperties = null)
        {
            // Add default properties for Quartz
            var defaults = new NameValueCollection
            {
                { "quartz.scheduler.instanceName"   , "Quartz Scheduler"                      },
                { "quartz.threadPool.type"          , "Quartz.Simpl.SimpleThreadPool, Quartz" },
                { "quartz.threadPool.threadCount"   , "4"                                     },
                { "quartz.jobStore.misfireThreshold", "60000"                                 },
                { "quartz.serializer.type"          , "binary"                                }
                                                                                              };
            if (quartzProperties == null)
                quartzProperties = defaults;
            foreach (string key in defaults)
            {
                if (String.IsNullOrEmpty(quartzProperties.Get(key)))
                    quartzProperties.Add(key, defaults.Get(key));
            }

            // Add Quartz services
            services.AddSingleton<IJobFactory, SchedulerJobFactory>();
            services.AddSingleton<ISchedulerFactory>(new StdSchedulerFactory(quartzProperties));

            // Hosted Service
            services.AddHostedService<SchedulerHostedService>();

            // Jobs scheduler
            services.AddSingleton<IScheduler, Scheduler>();

            return services;
        }
    }
}
