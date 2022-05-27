using MediatR;

namespace SimpleApi;

public class Ping : INotification
{
    
}

public class PingHandler : INotificationHandler<Ping>
{
    private readonly ILogger<PingHandler> _logger;
    private readonly DummyService _service;
    
    public PingHandler(ILogger<PingHandler> logger, DummyService service)
    {
        _logger = logger;
        _service = service;
    }

    public async Task Handle(Ping notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Pong handler");
        await Task.Delay(13000, cancellationToken);
        
        _service.WhoAmI();
    }
}

public class DummyService
{
    private readonly ILogger<DummyService> _logger;

    public DummyService(ILogger<DummyService> logger)
    {
        _logger = logger;
    }

    public void WhoAmI()
    {
        _logger.LogDebug("I am {HashCode}", this.GetHashCode());
        throw new Exception("Unhandled exception");
    }
}
