using System;
using Autofac;
using Autofac.Features.Decorators;

namespace Decorators
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // Based on article
            // https://alexmg.com/posts/upcoming-decorator-enhancements-in-autofac-4-9
            var builder = new ContainerBuilder();
            builder.RegisterType<Service>().As<IService>();
            builder.RegisterDecorator<LoggingService, IService>();
            builder.RegisterDecorator<TransactionalService, IService>();

            var container = builder.Build();
            var service = container.Resolve<IService>();

            service.Do("This is a message!");
            Console.ReadLine();
        }
    }

    public interface IService { void Do(string message); }

    public class Service : IService
    {
        public void Do(string message) => Console.WriteLine($"Service: {message}");
    }

    public class LoggingService : IService
    {
        private readonly IService innerService;

        public LoggingService(IService innerService) => this.innerService = innerService;

        public void Do(string message)
        {
            Console.WriteLine($"Inside {nameof(LoggingService)}, before inner call.");
            innerService.Do(message);
            Console.WriteLine($"Inside {nameof(LoggingService)}, after inner call.");
        }
    }

    public class TransactionalService : IService
    {
        private readonly IService innerService;
        private readonly IDecoratorContext context;

        public TransactionalService(IService innerService, IDecoratorContext context)
        {
            this.innerService = innerService;
            this.context = context;
        }

        public void Do(string message)
        {
            Console.WriteLine($"Inside {nameof(TransactionalService)}, before inner call.");
            this.innerService.Do(message);
            Console.WriteLine($"Inside {nameof(TransactionalService)}, after inner call.");
        }
    }
}
