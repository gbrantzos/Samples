using MediatR;

namespace SimpleApi
{
    public abstract class Request<TResponse> : IRequest<Result<TResponse, Error>>
    {
    
    }

    public class Command<TResponse> : Request<TResponse>
    {
    
    }

    public class Query<TResponse> : Request<TResponse>
    {
    
    }
}
