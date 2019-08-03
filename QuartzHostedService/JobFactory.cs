using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;

namespace QuartzHostedService
{
    public class JobFactory : IJobFactory
    {
        private readonly IServiceScopeFactory scopeFactory;
        private IServiceScope scope;

        public JobFactory(IServiceScopeFactory scopeFactory) 
            => this.scopeFactory = scopeFactory;

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            scope = scopeFactory.CreateScope();
            return scope.ServiceProvider.GetService(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job) => scope?.Dispose();
    }
}
