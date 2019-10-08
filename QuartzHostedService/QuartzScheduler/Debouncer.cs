using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace QuartzHostedService.QuartzScheduler
{
    // Based on https://gist.github.com/cocowalla/5d181b82b9a986c6761585000901d1b8

    public class Debouncer : IDisposable
    {
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private readonly TimeSpan waitTime;
        private int counter;

        public Debouncer(TimeSpan? waitTime = null)
        {
            this.waitTime = waitTime ?? TimeSpan.FromSeconds(3);
        }

        public void Debouce(Action action)
        {
            var current = Interlocked.Increment(ref this.counter);

            Task.Delay(this.waitTime).ContinueWith(task =>
            {
                // Is this the last task that was queued?
                if (current == this.counter && !this.cts.IsCancellationRequested)
                    action();

                task.Dispose();
            }, this.cts.Token);
        }

        public void Dispose()
        {
            this.cts.Cancel();
        }
    }

    public static class IConfigurationExtensions
    {
        /// <summary>
        /// Perform an action when configuration changes. Note this requires configuration sources to be added with
        /// `reloadOnChange` enabled
        /// </summary>
        /// <param name="config">Configuration to watch for changes</param>
        /// <param name="action">Action to perform when <paramref name="config"/> is changed</param>
        public static void OnChange(this IConfiguration config, Action action)
        {
            // IConfiguration's change detection is based on FileSystemWatcher, which will fire multiple change
            // events for each change - Microsoft's code is buggy in that it doesn't bother to debounce/dedupe
            // https://github.com/aspnet/AspNetCore/issues/2542
            var debouncer = new Debouncer(TimeSpan.FromSeconds(2));

            ChangeToken.OnChange<object>(config.GetReloadToken, _ => debouncer.Debouce(action), null);
        }
    }
}
