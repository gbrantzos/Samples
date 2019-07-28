using Microsoft.Extensions.Options;

namespace QuartzSample
{
    public interface IDemoService
    {
        string Execute(string message);
    }

    public class DemoService : IDemoService
    {
        private readonly DemoServiceConfig config;

        public DemoService(IOptionsMonitor<DemoServiceConfig> options)
            => this.config = options.CurrentValue;

        public string Execute(string message)
            => $"Received: {message}, will be sent to {config.Url}";

    }
}
