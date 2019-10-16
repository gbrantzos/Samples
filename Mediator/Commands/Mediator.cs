using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Mediator.Commands
{
    public class Mediator : IMediator
    {
        private readonly ServiceFactory serviceFactory;

        public Mediator(ServiceFactory serviceFactory)
            => this.serviceFactory = serviceFactory;

        public Task<TResult> HandleAsync<TResult>(ICommand<TResult> command,
            IExecutionContext context = null,
            CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (context == null)
                context = ExecutionContext.Default;

            var commandType = command.GetType();
            var handlerType = typeof(IHandler<,>).MakeGenericType(commandType, typeof(TResult));

            var handlerInstance = this.serviceFactory(handlerType);
            return InvokeHandle(handlerInstance, command, context, cancellationToken);
        }

        private Task<TResult> InvokeHandle<TResult>(object handlerInstance,
            ICommand<TResult> command,
            IExecutionContext context,
            CancellationToken cancellationToken)
        {
            var method = handlerInstance
              .GetType()
              .GetTypeInfo()
              .GetMethod(nameof(IHandler<ICommand<TResult>, TResult>.Handle));

            if (method == null)
                throw new ArgumentException($"Could not locate handler for command type {command.GetType().Name}");

            return (Task<TResult>)method.Invoke(handlerInstance, new object[] { command, context, cancellationToken });
        }
    }
}
