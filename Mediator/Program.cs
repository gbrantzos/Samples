using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Mediator.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            //await new MediatorSample().Run();
            var serviceBuilder = new ServiceCollection()
                .AddScoped<ServiceFactory>(p => p.GetService)
                .AddScoped<IMediator, Commands.Mediator>()
                .AddTransient(typeof(IHandler<SimpleCommand, bool>), typeof(SimpleCommandHandler));
            var serviceProvider = serviceBuilder.BuildServiceProvider();

            var mediator = serviceProvider.GetService<IMediator>();
            var command = new SimpleCommand { ExecutionID = 32 };

            //var summary = BenchmarkRunner.Run<MediatorSample>();
            var sw = new Stopwatch();
            sw.Start();
            int length = 5000000;
            for (int i = 0; i < length; i++)
            {
                var result = await mediator.HandleAsync(command);
            }
            sw.Stop();
            Console.WriteLine($"{length} executions, elapsed time: {sw.ElapsedMilliseconds}ms");
            Console.ReadKey();
        }
    }

    [RPlotExporter, RankColumn]
    public class MediatorSample
    {
        [Benchmark]
        public async Task Run()
        {
            var serviceBuilder = new ServiceCollection()
                .AddScoped<ServiceFactory>(p => p.GetService)
                .AddScoped<IMediator, Commands.Mediator>()
                .AddTransient(typeof(IHandler<SimpleCommand, bool>), typeof(SimpleCommandHandler));
            var serviceProvider = serviceBuilder.BuildServiceProvider();

            var mediator = serviceProvider.GetService<IMediator>();
            var command = new SimpleCommand { ExecutionID = 32 };
            var result = await mediator.HandleAsync(command);

            //Console.WriteLine($"Result was {result}");
            //Console.ReadLine();
        }
    }

    public class SimpleCommand : ICommand<bool>
    {
        public int ExecutionID { get; set; }

    }

    public class SimpleCommandHandler : IHandler<SimpleCommand, bool>
    {
        public Task<bool> Handle(SimpleCommand command, IExecutionContext context, CancellationToken cancellationToken)
        {
            //Console.WriteLine($"Executing: {command.ExecutionID}");
            return Task.FromResult(true);
        }
    }
}
