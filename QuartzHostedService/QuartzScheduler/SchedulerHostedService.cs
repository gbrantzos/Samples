using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace QuartzHostedService.QuartzScheduler
{
    public class SchedulerHostedService : IHostedService
    {
        private readonly IScheduler scheduler;

        public SchedulerHostedService(IScheduler scheduler)
            => this.scheduler = scheduler;

        public async Task StartAsync(CancellationToken cancellationToken)
            => await this.scheduler.Start(cancellationToken);

        public async Task StopAsync(CancellationToken cancellationToken)
            => await this.scheduler.Stop(cancellationToken);
    }
}
