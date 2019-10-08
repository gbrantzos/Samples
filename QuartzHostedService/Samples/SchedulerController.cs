using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuartzHostedService.QuartzScheduler;

namespace QuartzHostedService.Samples
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulerController : ControllerBase
    {
        private readonly IJobScheduler scheduler;

        public SchedulerController(IJobScheduler scheduler) => this.scheduler = scheduler;

        public async Task<SchedulerStatus> Get() => await scheduler.GetStatus();
    }
}