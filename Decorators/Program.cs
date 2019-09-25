using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Features.Decorators;
using MediatR;

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

            builder
              .RegisterType<Mediator>()
              .As<IMediator>()
              .InstancePerLifetimeScope();
            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            // Register handler
            builder.RegisterType<SimpleRequestHandler>().AsImplementedInterfaces().InstancePerDependency();

            // Register handler specific decorators
            builder.RegisterDecorator<EnhancedSimpleRequestHandler, IRequestHandler<SimpleRequest, bool>>();

            // Register generic decorators (will be the outer decorator, runs first and last!)
            builder.RegisterGenericDecorator(typeof(GenericRequestHandlerDecorator<,>), typeof(IRequestHandler<,>));

            var container = builder.Build();
            var service = container.Resolve<IService>();

            service.Do("This is a message!");

            var request = new SimpleRequest
            {
                ConnectionString = "there"
            };
            var mediatr = container.Resolve<IMediator>();
            var result = mediatr.Send(request).Result;
            Console.WriteLine($"Result was: '{result}'");

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

    public class SimpleRequest : IRequest<bool>
    {
        public string ConnectionString { get; set; }
    }

    public class SimpleRequestHandler : IRequestHandler<SimpleRequest, bool>
    {
        public Task<bool> Handle(SimpleRequest request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Base request handler {nameof(SimpleRequestHandler)}");
            return Task.FromResult(true);
        }
    }

    public class EnhancedSimpleRequestHandler : IRequestHandler<SimpleRequest, bool>
    {
        private readonly IRequestHandler<SimpleRequest, bool> inner;

        public EnhancedSimpleRequestHandler(IRequestHandler<SimpleRequest, bool> inner)
        {
            this.inner = inner;
        }

        public Task<bool> Handle(SimpleRequest request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Request - Response specific decorator {nameof(EnhancedSimpleRequestHandler)}");
            return this.inner.Handle(request, cancellationToken);
        }
    }

    public class GenericRequestHandlerDecorator<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> inner;
        private readonly IDecoratorContext context;

        public GenericRequestHandlerDecorator(IRequestHandler<TRequest, TResponse> inner, IDecoratorContext context)
        {
            this.inner = inner;
            this.context = context;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Generic decorator {nameof(GenericRequestHandlerDecorator<TRequest, TResponse>)}");
            return this.inner.Handle(request, cancellationToken);
        }
    }
}
