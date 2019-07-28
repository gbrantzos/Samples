using System;
using System.Threading.Tasks;
using Quartz;

namespace QuartzSample
{
    public class DemoJob : IJob
    {
        private readonly IDemoService service;

        public DemoJob(IDemoService service) => this.service = service;

        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine(service.Execute(DateTime.Now.ToLongTimeString()));
            return Task.CompletedTask;
        }
    }
}
