using MediatR;

namespace SimpleApi;

public abstract class Handler<TRequest, TResponse> : IRequestHandler<TRequest, Result<TResponse, Error>>
    where TRequest : Request<TResponse>
{
    public async Task<Result<TResponse, Error>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return await HandleCore(request, cancellationToken);
        }
        catch (Exception e)
        {
            return new SystemError(e);
        }
    }

    protected abstract Task<Result<TResponse, Error>> HandleCore(TRequest request, CancellationToken cancellationToken);
}
