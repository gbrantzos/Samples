using System.Threading;
using System.Threading.Tasks;

namespace QuartzHostedService.QuartzScheduler
{
    public interface IScheduler
    {
        /// <summary>
        /// Start scheduler.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Start(CancellationToken cancellationToken = default);

        /// <summary>
        /// Stop scheduler
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Stop(CancellationToken cancellationToken = default);

        /// <summary>
        /// Stop scheduler if active, reload configuration and start again.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Reload(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get current <see cref="SchedulerStatus"/> of <see cref="Scheduler"/>.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SchedulerStatus> GetStatus(CancellationToken cancellationToken = default);
    }
}
