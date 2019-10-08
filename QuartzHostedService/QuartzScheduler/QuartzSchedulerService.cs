using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Spi;

namespace QuartzHostedService.QuartzScheduler
{
    public class QuartzSchedulerService : IHostedService, IJobScheduler
    {
        private const string ConfigurationFileName = "SchedulerSettings.yaml";

        private readonly ISchedulerFactory schedulerFactory;
        private readonly IJobFactory jobFactory;
        private readonly ILogger<QuartzSchedulerService> logger;
        private readonly IConfigurationRoot schedulerConfiguration;

        public IScheduler Scheduler { get; set; }

        public QuartzSchedulerService(
            ISchedulerFactory schedulerFactory,
            IJobFactory jobFactory,
            ILogger<QuartzSchedulerService> logger)
        {
            this.schedulerFactory = schedulerFactory;
            this.jobFactory = jobFactory;
            this.logger = logger;

            var configurationPath = Path.Combine(
                Path.GetDirectoryName(typeof(QuartzSchedulerService).Assembly.Location),
                ConfigurationFileName);
            schedulerConfiguration = new ConfigurationBuilder()
                .AddYamlFile(configurationPath, optional: true, reloadOnChange: true)
                .Build();

            schedulerConfiguration.OnChange(async () => await Reload());
        }

        #region IJobScheduler methods
        public async Task Start(CancellationToken cancellationToken = default)
        {
            var jobKeys = await Scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
            int allJobs = jobKeys.Count;

            var triggers = await Scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup());
            int activeJobs = triggers.Count;

            logger.LogInformation($"Configuration loaded, {allJobs} jobs found, {activeJobs} active.");
            if (activeJobs == 0)
                logger.LogWarning("Scheduler will not be started [no active jobs].");
            else
            {
                await Scheduler.Start(cancellationToken);
                logger.LogInformation("Scheduler started!");
            }
        }

        public Task Stop(CancellationToken cancellationToken = default)
        {
            if (!Scheduler.InStandbyMode)
            {
                logger.LogInformation("Stopping scheduler...");
                return Scheduler.Standby(cancellationToken);
            }
            return Task.CompletedTask;
        }

        public async Task Reload(CancellationToken cancellationToken = default)
        {
            try
            {
                // Stop scheduler
                await Stop(cancellationToken);

                // Load configuration
                var settings = schedulerConfiguration
                        .GetSection(nameof(SchedulerSettings))
                        .Get<SchedulerSettings>();

                await Scheduler.Clear();
                foreach (var job in settings.Jobs)
                {
                    var quartzJob = job.GetQuartzJob();
                    var quartzTrigger = job.GetQuartzTrigger();
                    if (job.Active)
                        await Scheduler.ScheduleJob(quartzJob, quartzTrigger, cancellationToken);
                    else
                        await Scheduler.AddJob(quartzJob, true, true, cancellationToken);
                }

                // Start scheduler
                await Start(cancellationToken);
            }
            catch (Exception x)
            {
                logger.LogError(x, $"Failed to load scheduler settings: {x.Message}");
            }
        }

        public async Task<SchedulerStatus> GetStatus(CancellationToken cancellationToken = default)
        {
            var result = new SchedulerStatus
            {
                Active = Scheduler.IsStarted && !Scheduler.InStandbyMode,
                Jobs = new List<SchedulerJobStatus>()
            };
            var jobKeys = await Scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
            foreach (var key in jobKeys)
            {
                var details = await Scheduler.GetJobDetail(key);
                var triggers = await Scheduler.GetTriggersOfJob(key);
                var trigger = triggers.FirstOrDefault();
                var jobStatus = new SchedulerJobStatus
                {
                    Name                      = details.Description,
                    Active                    = trigger != null,
                    CronExpression            = trigger == null ? "-" : trigger.Description,
                    CronExpressionDescription = trigger == null ? "-" : trigger.Description,
                    PreviousFireTime          = trigger?.GetPreviousFireTimeUtc()?.ToLocalTime().DateTime,
                    NextFireTime              = trigger?.GetPreviousFireTimeUtc()?.ToLocalTime().DateTime
                };
                result.Jobs.Add(jobStatus);
            }
            return result;
        }
        #endregion

        #region IHostedService methods
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Prepare Scheduler
            Scheduler = await schedulerFactory.GetScheduler(cancellationToken);
            Scheduler.JobFactory = jobFactory;

            // Load and start
            await Reload(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
            => await Scheduler.Shutdown(cancellationToken);
        #endregion
    }

    // Based on https://gist.github.com/cocowalla/5d181b82b9a986c6761585000901d1b8
    public class Debouncer : IDisposable
    {
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private readonly TimeSpan waitTime;
        private int counter;

        public Debouncer(TimeSpan? waitTime = null)
        {
            this.waitTime = waitTime ?? TimeSpan.FromSeconds(3);
        }

        public void Debouce(Action action)
        {
            var current = Interlocked.Increment(ref this.counter);

            Task.Delay(this.waitTime).ContinueWith(task =>
            {
                // Is this the last task that was queued?
                if (current == this.counter && !this.cts.IsCancellationRequested)
                    action();

                task.Dispose();
            }, this.cts.Token);
        }

        public void Dispose()
        {
            this.cts.Cancel();
        }
    }

    public static class IConfigurationExtensions
    {
        /// <summary>
        /// Perform an action when configuration changes. Note this requires config sources to be added with
        /// `reloadOnChange` enabled
        /// </summary>
        /// <param name="config">Configuration to watch for changes</param>
        /// <param name="action">Action to perform when <paramref name="config"/> is changed</param>
        public static void OnChange(this IConfiguration config, Action action)
        {
            // IConfiguration's change detection is based on FileSystemWatcher, which will fire multiple change
            // events for each change - Microsoft's code is buggy in that it doesn't bother to debounce/dedupe
            // https://github.com/aspnet/AspNetCore/issues/2542
            var debouncer = new Debouncer(TimeSpan.FromSeconds(2));

            ChangeToken.OnChange<object>(config.GetReloadToken, _ => debouncer.Debouce(action), null);
        }
    }
}
