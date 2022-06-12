using System.Diagnostics;
using MediatR;
using Prometheus;

namespace SimpleApi;

public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse, Error>>
    where TRequest : Request<TResponse>
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<Result<TResponse, Error>> Handle(TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<Result<TResponse, Error>> next)
    {
        var requestType = typeof(TRequest).Name;
        var traceID = Activity.Current?.Id ?? _httpContextAccessor.HttpContext!.TraceIdentifier;
        var counter = Metrics.CreateCounter("simple_api_requests", "SimpleAPI requests", new[] {"request_type"});
        counter.WithLabels(requestType).Inc();
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["TraceID"] = traceID,
            ["Request"] = requestType
        });
        
        _logger.LogInformation("Executing request {RequestType}", requestType);
        var sw = new Stopwatch();
        sw.Start();
        
        var result = next();
        
        _logger.LogInformation("Execution time {ElapsedTime}ms", sw.ElapsedMilliseconds);
        sw.Stop();
        
        return result;
    }
}
