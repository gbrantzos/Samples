using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CronExpressionDescriptor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Spi;

namespace QuartzHostedService.QuartzScheduler
{
    public class Scheduler : IScheduler
    {
        // TODO Add storage for last executions and other data (Cron, Settings per Job)
        private const string ConfigurationFileName = "SchedulerSettings.yaml";

        private readonly IJobFactory jobFactory;
        private readonly ISchedulerFactory schedulerFactory;
        private readonly IConfigurationRoot schedulerConfiguration;
        private readonly ILogger<Scheduler> logger;
        private Quartz.IScheduler quartzScheduler;

        public Scheduler(
            IJobFactory jobFactory,
            ISchedulerFactory schedulerFactory,
            ILogger<Scheduler> logger)
        {
            this.schedulerFactory = schedulerFactory;
            this.jobFactory = jobFactory;
            this.logger = logger;

            var configurationPath = Path.Combine(
                Path.GetDirectoryName(typeof(Scheduler).Assembly.Location),
                ConfigurationFileName);
            schedulerConfiguration = new ConfigurationBuilder()
                .AddYamlFile(configurationPath, optional: true, reloadOnChange: true)
                .Build();

            schedulerConfiguration.OnChange(async () => await Reload());
        }

        #region IJobScheduler methods
        public async Task Start(CancellationToken cancellationToken = default)
        {
            if (quartzScheduler == null)
            {
                // First time initialization of Scheduler
                quartzScheduler = await schedulerFactory.GetScheduler(cancellationToken);
                quartzScheduler.JobFactory = jobFactory;
                quartzScheduler.ListenerManager.AddJobListener(new JobListener());

                // Load configuration
                await LoadConfiguration(cancellationToken);
            }

            var jobKeys = await quartzScheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
            int allJobs = jobKeys.Count;

            var triggers = await quartzScheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup());
            int activeJobs = triggers.Count;

            logger.LogInformation($"Configuration loaded, {allJobs} jobs found, {activeJobs} active.");
            if (activeJobs == 0)
                logger.LogWarning("Scheduler will not be started [no active jobs].");
            else
            {
                await quartzScheduler.Start(cancellationToken);
                logger.LogInformation("Scheduler started!");
            }
        }

        public Task Stop(CancellationToken cancellationToken = default)
        {
            if (!quartzScheduler.InStandbyMode)
            {
                logger.LogInformation("Stopping scheduler...");
                return quartzScheduler.Standby(cancellationToken);
            }
            return Task.CompletedTask;
        }

        public async Task Reload(CancellationToken cancellationToken = default)
        {
            await Stop(cancellationToken);
            await LoadConfiguration(cancellationToken);
            await Start(cancellationToken);
        }

        public async Task<SchedulerStatus> GetStatus(CancellationToken cancellationToken = default)
        {
            var result = new SchedulerStatus
            {
                Active = quartzScheduler.IsStarted && !quartzScheduler.InStandbyMode,
                Jobs = new List<SchedulerJobStatus>()
            };
            var jobKeys = await quartzScheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
            foreach (var key in jobKeys)
            {
                var details        = await quartzScheduler.GetJobDetail(key);
                var triggers       = await quartzScheduler.GetTriggersOfJob(key);
                var trigger        = triggers.FirstOrDefault();
                // TODO Remove that smell!
                var tmp            = details.Description.Split('@', StringSplitOptions.RemoveEmptyEntries);
                var name           = tmp[0].Trim();
                var cronExpression = tmp.Length > 1 ? tmp[1].Trim() : String.Empty;

                var jobStatus = new SchedulerJobStatus
                {
                    Name                      = name,
                    Active                    = trigger != null,
                    Jobtype                   = details.JobType.Name,
                    CronExpression            = cronExpression,
                    CronExpressionDescription = cronExpression,
                    PreviousFireTime          = trigger?.GetPreviousFireTimeUtc()?.ToLocalTime().DateTime,
                    NextFireTime              = trigger?.GetNextFireTimeUtc()?.ToLocalTime().DateTime
                };
                if (!String.IsNullOrEmpty(jobStatus.CronExpression))
                    jobStatus.CronExpressionDescription = ExpressionDescriptor.GetDescription(jobStatus.CronExpression);
                result.Jobs.Add(jobStatus);
            }
            return result;
        }
        #endregion

        // Helpers
        private async Task LoadConfiguration(CancellationToken cancellationToken = default)
        {
            try
            {
                if (!quartzScheduler.InStandbyMode)
                    throw new InvalidOperationException($"{nameof(LoadConfiguration)} should be called when {nameof(Scheduler)} is stopped.");

                // Load configuration
                var settings = schedulerConfiguration
                        .GetSection(nameof(SchedulerSettings))
                        .Get<SchedulerSettings>();

                await quartzScheduler.Clear();
                foreach (var job in settings.Jobs)
                {
                    var quartzJob = job.GetQuartzJob();
                    var quartzTrigger = job.GetQuartzTrigger();
                    if (job.Active)
                        await quartzScheduler.ScheduleJob(quartzJob, quartzTrigger, cancellationToken);
                    else
                        await quartzScheduler.AddJob(quartzJob, true, true, cancellationToken);
                }
            }
            catch (Exception x)
            {
                logger.LogError(x, $"Failed to load scheduler settings: {x.Message}");
            }
        }

    }

    class JobListener : IJobListener
    {
        public string Name => "Job Listener";

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

}
