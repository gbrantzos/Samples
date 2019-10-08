using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Quartz;
using Quartz.Spi;

namespace QuartzHostedService
{
    public class QuartzSchedulerService : IHostedService
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

            schedulerConfiguration.OnChange(async () => await LoadConfiguration());
        }

        private async Task LoadConfiguration(CancellationToken cancellationToken = default)
        {
            try
            {
                var settings = schedulerConfiguration
                        .GetSection(nameof(SchedulerSettings))
                        .Get<SchedulerSettings>();

                if (!Scheduler.InStandbyMode)
                {
                    logger.LogInformation("Stopping scheduler...");
                    await Scheduler.Standby(cancellationToken);
                }

                await Scheduler.Clear();
                foreach (var job in settings.Jobs)
                {
                    var quartzJob     = job.GetQuartzJob();
                    var quartzTrigger = job.GetQuartzTrigger();
                    if (job.Active)
                        await Scheduler.ScheduleJob(quartzJob, quartzTrigger, cancellationToken);
                    else
                        await Scheduler.AddJob(quartzJob, true, true, cancellationToken);
                }

                int activeJobs = settings.Jobs.Count(job => job.Active);
                logger.LogInformation($"Configuration loaded, {settings.Jobs.Count} jobs found, {activeJobs} active.");
                if (activeJobs == 0)
                    logger.LogWarning("Scheduler will not be started [no active jobs].");
                else
                {
                    await Scheduler.Start(cancellationToken);
                    logger.LogInformation("Scheduler started!");
                }
            }
            catch (Exception x)
            {
                logger.LogError(x, $"Failed to load scheduler settings: {x.Message}");
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Prepare Scheduler
            Scheduler = await schedulerFactory.GetScheduler(cancellationToken);
            Scheduler.JobFactory = jobFactory;

            // Load and start
            await LoadConfiguration(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
            => await Scheduler.Shutdown(cancellationToken);
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
