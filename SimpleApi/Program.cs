using Autofac.Extensions.DependencyInjection;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.OpenApi.Models;
using Prometheus;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using SimpleApi;


var thisAssembly = typeof(Program).Assembly;
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(new CompactJsonFormatter(),
        "log.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7)
    .Enrich.WithExceptionDetails()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Support YAML settings
    builder.Host.ConfigureAppConfiguration((host, config) =>
    {
        var jsonSources = config.Sources.Where(s => s is JsonConfigurationSource).ToList();
        foreach (var source in jsonSources)
            config.Sources.Remove(source);

        var env = host.HostingEnvironment;
        config.AddYamlFile("appsettings.yaml", optional: true, reloadOnChange: true);
        config.AddYamlFile($"appsettings.{env.EnvironmentName}.yaml", optional: true, reloadOnChange: true);
    });

    // Use Autofac
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

    // Wire up Serilog
    builder.Host.UseSerilog((ctx, services, config) => config
        .ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(services));

    // Swagger
    builder.Services
        .AddEndpointsApiExplorer()
        .AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo {Title = "Simple API", Version = "v1"});
            c.EnableAnnotations();
            c.IgnoreObsoleteActions();
        });

    // Setup controllers
    builder.Services
        .AddControllers()
        .AddControllersAsServices();

    // Setup Mediator
    builder.Services
        .AddMediatR(thisAssembly)
        .AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>))
        .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

    // Setup FluentValidation
    builder.Services.AddValidatorsFromAssembly(thisAssembly);

    // Other services
    builder.Services
        .AddProblemDetails(options =>
        {
            // Control when an exception is included
            options.IncludeExceptionDetails = (ctx, _) =>
            {
                // Fetch services from HttpContext.RequestServices
                var env = ctx.RequestServices.GetRequiredService<IHostEnvironment>();
                return env.IsDevelopment() || env.IsStaging();
            };
        })
        .AddHttpContextAccessor()
        .AddScoped<DummyService>();

    // Build app and run
    var app = builder.Build();

    // Http logging
    // Relevant logger: Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware
    // https://josef.codes/asp-net-core-6-http-logging-log-requests-responses/
    app.UseHttpLogging();
    app.UseSerilogRequestLogging();

    app.UseProblemDetails();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.DefaultModelsExpandDepth(-1)); // Disable swagger schemas at bottom
    app.UseHttpMetrics();

    app.MapControllers();
    app.MapGet("/", () => $"Welcome to SimpleAPI\n\n{BuildInformation.Instance.ToDisplayString()}")
        .ExcludeFromDescription();
    app.MapMetrics();

    // Sample middleware accessing endpoint information
    /*
    app.Use(async (context, func) =>
    {
        var route = context.GetEndpoint();
        await func();
    });
    */

    app.Run();
}
catch (Exception x)
{
    Log.Error(x, "Unhandled exception: {ErrorMessage}", x.Message);
#if DEBUG
    // Give a last chance to view exception
    Console.WriteLine("Press eny key to exit...");
    Console.ReadKey(true);
#endif
}
finally
{
    Log.CloseAndFlush();
}
