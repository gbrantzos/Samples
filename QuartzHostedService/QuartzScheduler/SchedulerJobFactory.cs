using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;

namespace QuartzHostedService
{
    public class SchedulerJobFactory : IJobFactory
    {
        private readonly Dictionary<int, IServiceScope> scopes = new Dictionary<int, IServiceScope>();
        private readonly IServiceScopeFactory scopeFactory;

        public SchedulerJobFactory(IServiceScopeFactory scopeFactory)
            => this.scopeFactory = scopeFactory;

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var scope = scopeFactory.CreateScope();
            var job   = scope.ServiceProvider.GetService(bundle.JobDetail.JobType) as IJob;
            this.scopes.Add(job.GetHashCode(), scope);

            return job;
        }

        public void ReturnJob(IJob job)
        {
            var key = job.GetHashCode();
            if (scopes.TryGetValue(key, out var scope))
            {
                (job as IDisposable)?.Dispose();
                scope?.Dispose();
                scopes.Remove(key);
            }
        }
    }
}
