using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QuartzHostedService.QuartzScheduler
{
    public interface IJobScheduler
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

        Task<SchedulerStatus> GetStatus(CancellationToken cancellationToken = default);
    }

    public class SchedulerStatus
    {
        public bool Active { get; set; }
        public List<SchedulerJobStatus> Jobs { get; set; }
    }

    public class SchedulerJobStatus
    {
        public string Name { get; set; }
        public bool Active { get; set; }
        public string CronExpression { get; set; }
        public string CronExpressionDescription { get; set; }
        public string Jobtype { get; set; }
        public DateTime? PreviousFireTime { get; set; }
        public DateTime? NextFireTime { get; set; }
    }
}
