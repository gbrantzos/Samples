using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;

namespace QuartzHostedService.Samples
{
    [DisallowConcurrentExecution]
    public class SecondJob : IJob
    {
        private readonly ILogger<SecondJob> logger;

        public SecondJob(ILogger<SecondJob> logger)
            => this.logger = logger;

        public Task Execute(IJobExecutionContext context)
        {
            logger.LogInformation("Second Job executed at {executionTime}", DateTime.Now);

            return Task.CompletedTask;
        }
    }
}
