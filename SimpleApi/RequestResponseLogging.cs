namespace SimpleApi;

public class RequestResponseLogItem
{
    public string RoutingKey { get; set; } = String.Empty;
    public string Path { get; set; } = String.Empty;
    public string QueryString { get; set; } = String.Empty;
    public string Method { get; set; } = String.Empty;
    public string Payload { get; set; } = String.Empty;
    public string? Response { get; set; }
    public string ResponseCode { get; set; } = String.Empty;
    public DateTime RequestedOn { get; set; }
    public DateTime RespondedOn { get; set; }
    public bool IsSuccessStatusCode => ResponseCode == "200" || ResponseCode == "201";
}

// This should replace "Http logging" (Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware) 
// https://josef.codes/asp-net-core-6-http-logging-log-requests-responses/
// app.UseHttpLogging();

// Ideas found on 
// https://elanderson.net/2019/12/log-requests-and-responses-in-asp-net-core-3/
// https://referbruv.com/blog/logging-http-request-and-response-in-aspnet-core-using-middlewares/
// https://exceptionnotfound.net/using-middleware-to-log-requests-and-responses-in-asp-net-core/

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RequestResponseLoggingOptions _options;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next,
        ILoggerFactory loggerFactory,
        RequestResponseLoggingOptions options)
    {
        _next = next;
        _options = options;
        _logger = loggerFactory.CreateLogger<RequestResponseLoggingMiddleware>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!ShouldLog(context))
        {
            await _next(context);
            return;
        }

        var endpoint = context.GetEndpoint() as RouteEndpoint;
        var routingKey = endpoint?.RoutePattern?.RawText;
        
        // Get request details
        var logItem = new RequestResponseLogItem
        {
            RoutingKey = routingKey ?? context.Request.Path,
            Path = context.Request.Path,
            Method = context.Request.Method,
            QueryString = context.Request.QueryString.ToString(),
            RequestedOn = DateTime.UtcNow
        };
        if ((context.Request.ContentLength ?? 0) > 0)
        {
            context.Request.EnableBuffering();
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;
            logItem.Payload = body;
        }

        // Prepare for stealing response
        await using (Stream originalRequest = context.Response.Body)
        {
            try
            {
                using var memStream = new MemoryStream();
                context.Response.Body = memStream;

                // Call next
                await _next(context);

                //We need to read the response stream from the beginning
                memStream.Position = 0;

                // Read the memory stream till the end
                var response = await new StreamReader(memStream).ReadToEndAsync();

                // Write the response to the log object
                logItem.Response = response;
                logItem.ResponseCode = context.Response.StatusCode.ToString();
                logItem.RespondedOn = DateTime.UtcNow;

                // We have reached the end of the stream, reset it onto the first position
                memStream.Position = 0;

                // Copy the content of the temporary memory stream we have passed to the actual response body 
                await memStream.CopyToAsync(originalRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process response");
            }
            finally
            {
                // Assign the response body to the actual context
                context.Response.Body = originalRequest;
            }
        }

        // Finally log...
        _logger.LogInformation("Advanced HTTP logging\n\n{@LogItem}\n", logItem);
    }

    private bool ShouldLog(HttpContext context)
    {
        var path = context.Request.Path.Value ?? String.Empty;
        var method = context.Request.Method;

        return _options.IsIncluded(path, method) && !_options.IsExcluded(path, method);
    }
}

public class RequestResponseLoggingOptions
{
    private readonly List<(string Path, string Method)> _includeCriteria = new();
    private readonly List<(string Path, string Method)> _excludeCriteria = new();

    public void IncludePath(string path, string method = "*") => _includeCriteria.Add((path, method));
    public void ExcludePath(string path, string method = "*") => _excludeCriteria.Add((path, method));

    public bool IsIncluded(string path, string method)
    {
        if (_includeCriteria.Count == 0)
            return true;

        return _includeCriteria
            .Any(criterion => path.StartsWith(criterion.Path) && (method == criterion.Method || criterion.Method == "*"));
    }

    public bool IsExcluded(string path, string method)
    {
        if (_excludeCriteria.Count == 0)
            return false;

        return _excludeCriteria
            .Any(criterion => path.StartsWith(criterion.Path) && (method == criterion.Method || criterion.Method == "*"));
    }
}

public static class RequestResponseLoggingExtensions
{
    public static IApplicationBuilder UseRequestResponseLogging(
        this IApplicationBuilder applicationBuilder,
        Action<RequestResponseLoggingOptions>? configure = null)
    {
        var options = new RequestResponseLoggingOptions();
        configure?.Invoke(options);
        applicationBuilder.UseMiddleware<RequestResponseLoggingMiddleware>(options);

        return applicationBuilder;
    }
}
