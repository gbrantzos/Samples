using System.Threading;
using System.Threading.Tasks;

namespace Mediator.Commands
{
    public interface IMediator
    {
        Task<TResult> HandleAsync<TResult>(ICommand<TResult> message,
            IExecutionContext context = default(IExecutionContext),
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
