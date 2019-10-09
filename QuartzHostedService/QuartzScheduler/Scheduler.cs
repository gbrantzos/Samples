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
        private const string ConfigurationFileName = "SchedulerSettings.yaml";

        private class SchedulerJobDetails
        {
            public string Name { get; set; }
            public string CronExpression { get; set; }
            public JobKey JobKey { get; set; }
            public Exception LastExecutionException { get; set; }
        }

        private readonly IJobFactory jobFactory;
        private readonly ISchedulerFactory schedulerFactory;
        private readonly IConfigurationRoot schedulerConfiguration;
        private readonly ILogger<Scheduler> logger;
        private ICollection<SchedulerJobDetails> internalDetails = new HashSet<SchedulerJobDetails>();

        private Quartz.IScheduler quartzScheduler;
        private bool failedToLoad;

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
                var initialized = await InitialiseScheduler(cancellationToken);
                if (!initialized)
                    throw new InvalidOperationException("Failed to initialize scheduler.");
            }

            var jobKeys = await quartzScheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
            int allJobs = jobKeys.Count;

            var triggers = await quartzScheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup());
            int activeJobs = triggers.Count;

            logger.LogInformation($"Configuration loaded, {allJobs} jobs found, {activeJobs} active.");
            if (activeJobs == 0)
                logger.LogWarning("Scheduler will be started, but has no active jobs!");
            else
                logger.LogInformation("Scheduler started.");
            await quartzScheduler.Start(cancellationToken);
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
            var loaded = await LoadConfiguration(cancellationToken);
            if (loaded)
                await Start(cancellationToken);
        }

        public async Task<SchedulerStatus> GetStatus(CancellationToken cancellationToken = default)
        {
            var result = new SchedulerStatus { Jobs = new List<SchedulerJobStatus>() };
            if (failedToLoad)
            {
                result.SchedulerState = SchedulerStatus.State.FailedToLoad;
                return result;
            }

            if (quartzScheduler.IsStarted)
                result.SchedulerState = SchedulerStatus.State.Active;
            if (quartzScheduler.InStandbyMode)
                result.SchedulerState = SchedulerStatus.State.StandBy;

            var jobKeys = await quartzScheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
            foreach (var key in jobKeys)
            {
                var jobDetail      = await quartzScheduler.GetJobDetail(key);
                var triggers       = await quartzScheduler.GetTriggersOfJob(key);
                var trigger        = triggers.FirstOrDefault();

                var internalDetail = internalDetails.First(dt => dt.JobKey.Equals(jobDetail.Key));
                var name           = internalDetail.Name;
                var cronExpression = internalDetail.CronExpression;

                var jobStatus = new SchedulerJobStatus
                {
                    Name                      = name,
                    Active                    = trigger != null,
                    JobType                   = jobDetail.JobType.Name,
                    CronExpression            = cronExpression,
                    PreviousFireTime          = trigger?.GetPreviousFireTimeUtc()?.ToLocalTime().DateTime,
                    NextFireTime              = trigger?.GetNextFireTimeUtc()?.ToLocalTime().DateTime
                };
                if (!String.IsNullOrEmpty(jobStatus.CronExpression))
                    jobStatus.CronExpressionDescription = ExpressionDescriptor.GetDescription(jobStatus.CronExpression);
                if (internalDetail.LastExecutionException != null)
                {
                    var allMesages = internalDetail
                        .LastExecutionException
                        .FromHierarchy(x => x.InnerException)
                        .Select(x => x.Message)
                        .Distinct()
                        .ToList();
                    jobStatus.FailureMessage = String.Join(Environment.NewLine, allMesages);
                }

                result.Jobs.Add(jobStatus);
            }
            return result;
        }

        public async Task ExecuteJob(string name, CancellationToken cancellationToken = default)
        {
            var internalDetail = internalDetails
                .FirstOrDefault(dt => dt.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            if (internalDetail == null)
                throw new ArgumentException($"Unknown job name: {name}");

            var jobDetail = await quartzScheduler.GetJobDetail(internalDetail.JobKey);
            var jobData = new JobDataMap();
            if (jobDetail.JobDataMap != null)
                jobData = (JobDataMap)jobDetail.JobDataMap.Clone();

            await quartzScheduler.TriggerJob(
                new JobKey(jobDetail.Key.Name, jobDetail.Key.Group),
                jobData,
                cancellationToken);
        }
        #endregion

        // Helpers
        private async Task<bool> InitialiseScheduler(CancellationToken cancellationToken = default)
        {
            // First time initialization of Scheduler
            quartzScheduler = await schedulerFactory.GetScheduler(cancellationToken);
            quartzScheduler.JobFactory = jobFactory;
            quartzScheduler.ListenerManager.AddJobListener(new JobListener(this));

            // Load configuration
            return await LoadConfiguration(cancellationToken);
        }

        private async Task<bool> LoadConfiguration(CancellationToken cancellationToken = default)
        {
            failedToLoad = true;
            try
            {
                if (!quartzScheduler.InStandbyMode)
                    throw new InvalidOperationException($"{nameof(LoadConfiguration)} should be called when {nameof(Scheduler)} is stopped.");

                // Load configuration
                var settings = schedulerConfiguration
                        .GetSection(nameof(SchedulerSettings))
                        .Get<SchedulerSettings>();
                await quartzScheduler.Clear();
                internalDetails = new HashSet<SchedulerJobDetails>();

                // Validate settings
                settings.Validate();

                foreach (var job in settings.Jobs)
                {
                    var details = new SchedulerJobDetails
                    {
                        Name = job.Name,
                        CronExpression = job.CronExpression,
                    };

                    var quartzJob = job.GetQuartzJob();
                    var quartzTrigger = job.GetQuartzTrigger();
                    if (job.Active)
                        await quartzScheduler.ScheduleJob(quartzJob, quartzTrigger, cancellationToken);
                    else
                        await quartzScheduler.AddJob(quartzJob, true, true, cancellationToken);

                    details.JobKey = quartzJob.Key;
                    internalDetails.Add(details);
                }

                failedToLoad = false;
                return true;
            }
            catch (Exception x)
            {
                logger.LogError(x, $"Failed to load scheduler settings: {x.Message}");
                return false;
            }
        }

        internal void AddExecutionResults(JobKey jobKey, Exception exception)
        {
            var internalDetail = internalDetails
                .FirstOrDefault(dt => dt.JobKey.Equals(jobKey));
            if (internalDetail == null)
                throw new ArgumentException($"Invalid job key: {jobKey}");

            internalDetail.LastExecutionException = exception;
        }

        private class JobListener : IJobListener
        {
            private readonly Scheduler scheduler;

            public JobListener(Scheduler scheduler) => this.scheduler = scheduler;

            public string Name => "Internal Job Listener";

            public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
                => Task.CompletedTask;

            public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
                => Task.CompletedTask;

            public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
            {
                scheduler.AddExecutionResults(context.JobDetail.Key, jobException);
                return Task.CompletedTask;
            }
        }
    }
}
