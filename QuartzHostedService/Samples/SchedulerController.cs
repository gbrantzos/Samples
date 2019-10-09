using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuartzHostedService.QuartzScheduler;

namespace QuartzHostedService.Samples
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulerController : ControllerBase
    {
        private readonly IScheduler scheduler;

        public SchedulerController(IScheduler scheduler) => this.scheduler = scheduler;

        public async Task<SchedulerStatus> Get() => await scheduler.GetStatus();

        [HttpPost("execute/{name}")]
        public async Task<IActionResult> Execute(string name)
        {
            try
            {
                await scheduler.ExecuteJob(name);
                return Ok(new { Message = $"Job executed", Name = name });
            }
            catch (ArgumentException x)
            {
                return NotFound(x.Message);
            }
        }
    }
}