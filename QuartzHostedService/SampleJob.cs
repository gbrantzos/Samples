using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace QuartzHostedService
{
    [DisallowConcurrentExecution]
    public class SampleJob : IJob
    {
        public SampleSettings settings { get; }

        private readonly ILogger<SampleJob> logger;
        public SampleJob(ILogger<SampleJob> logger, IOptionsSnapshot<SampleSettings> snapshot)
        {
            this.settings = snapshot.Value;
            this.logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            logger.LogInformation("Sample Job executed at {executionTime}, using {customerID}...", DateTime.Now, settings.CustomerID);
            return Task.CompletedTask;
        }
    }
}
