using MediatR;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection()  
    .AddMediatR(config =>
    {
        config.RegisterServicesFromAssemblyContaining<Program>();
        config.AddOpenBehavior(typeof(MediatrContextBehavior<,>));
    })
    .AddScoped<MediatrContext>()
    .BuildServiceProvider();

using var scope = services.CreateScope();
var request = new SampleRequest();
var sender = scope.ServiceProvider.GetRequiredService<ISender>();
var response = await sender.Send(request);
    
Console.WriteLine(response);


public record SampleRequest : IRequest<string>;

public class SampleHandler : IRequestHandler<SampleRequest, string>
{
    private readonly ISender _sender;
    public SampleHandler(ISender sender) => _sender = sender;

    public async Task<string> Handle(SampleRequest request, CancellationToken cancellationToken)
    {
        Console.WriteLine("Inside handler");
        var res = await _sender.Send(new NestedRequest(), cancellationToken);
        return $"Response - {res}";
    }
}

public record NestedRequest : IRequest<string>;

public class NestedHandler : IRequestHandler<NestedRequest, string>
{
    public Task<string> Handle(NestedRequest request, CancellationToken cancellationToken)
    {
        Console.WriteLine("Inside nested handler");
        return Task.FromResult("Inner Response");
    }
}


public class MediatrContextBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly MediatrContext _context;
    public MediatrContextBehavior(MediatrContext context) => _context = context;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _context.AddDepth();
        Console.WriteLine($"{typeof(TRequest).Name}");
        Console.WriteLine($"Before handler - {_context.Depth}");
        var result = await next();
        Console.WriteLine($"After handler - {_context.Depth}");

        return result;
    }
}

public class MediatrContext
{
    public int Depth { get; private set; }

    public void AddDepth() => Depth++;
}