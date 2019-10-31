using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sandbox
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var processor = new MyProcessor();
            var workItems = Enumerable
                .Range(1, 40)
                .Select(i => $"Payload of item number {i}")
                .ToList();
            processor.AddRange(workItems);

            Console.WriteLine("Hi!");
            Console.ReadLine();
        }
    }

    public class MyProcessor : WorkItemProcessor<string>
    {
        public MyProcessor() : base(5) { }

        protected override async Task Process(WorkItem<string> workItem, int worker)
        {
            await Task.Delay(40);
            Console.WriteLine($"Worker: {worker}, {workItem.Payload}, {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
