using System;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;

namespace QuartzSample
{
    public class JobFactory : IJobFactory
    {
        private readonly IServiceProvider serviceProvider;
        private IServiceScope scope;

        public JobFactory(IServiceProvider serviceProvider)
            => this.serviceProvider = serviceProvider;

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            scope = serviceProvider.CreateScope();
            return scope.ServiceProvider.GetService(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
            (job as IDisposable)?.Dispose();
            scope?.Dispose();
        }
    }
}
