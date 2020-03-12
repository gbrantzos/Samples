using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ContactManager.API.Middleware
{
    public class RequestLoggingMiddlewareOptions
    {
        // Placeholder class, might be needed in the future
    }

    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly RequestLoggingMiddlewareOptions options;
        private readonly ILogger<RequestLoggingMiddleware> logger;

        public RequestLoggingMiddleware(RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger,
            RequestLoggingMiddlewareOptions options)
        {
            this.next = next;
            this.logger = logger;
            this.options = options;
            this.options = options;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var sw = Stopwatch.StartNew();

            try
            {
                await next(context);
                sw.Stop();

                var elapsedMs  = sw.ElapsedTicks;
                var statusCode = context.Response.StatusCode;

                LogRequest(context, elapsedMs);
            }
            catch(Exception)
            {
                sw.Stop();
                LogRequest(context, sw.ElapsedTicks);

                throw;
            }
        }

        private void LogRequest(HttpContext context, long ticks)
        {
            static string UpperCaseFirst(string text)
                => Char.ToUpper(text[0]) + text.Substring(1);

            var req = context.Request;
            var res = context.Response;
            var ts  = new TimeSpan(ticks);
            var tsString = ts.TotalMinutes >= 1 ?
                    $"{(int)ts.TotalMinutes}m {ts:s\\.ff}s" :
                    ts.TotalSeconds > 1 ? $"{ts:s\\.fff}s" : $"{ts:fff}ms";

            var message = $"{UpperCaseFirst(req.Scheme)} {req.Method} {req.Path} responded {res.StatusCode} in {tsString}";
            logger.LogInformation(message);
        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder applicationBuilder,
            Action<RequestLoggingMiddlewareOptions> configureOptions = null)
        {
            var options = new RequestLoggingMiddlewareOptions();
            configureOptions?.Invoke(options);
            applicationBuilder.UseMiddleware<RequestLoggingMiddleware>(options);

            return applicationBuilder;
        }
    }
}
