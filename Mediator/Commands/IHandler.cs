using System.Threading;
using System.Threading.Tasks;

namespace Mediator.Commands
{
    public interface IHandler<in TCommand, TResult> where TCommand: ICommand<TResult>
    {
        Task<TResult> Handle(TCommand command, IExecutionContext context, CancellationToken cancellationToken);
    }
}
