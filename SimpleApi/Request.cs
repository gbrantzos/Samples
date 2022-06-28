using MediatR;

namespace SimpleApi
{
    public abstract record Request<TResponse> : IRequest<Result<TResponse, Error>> { }

    public record Command<TResponse> : Request<TResponse> { }

    public record Query<TResponse> : Request<TResponse> { }
}
