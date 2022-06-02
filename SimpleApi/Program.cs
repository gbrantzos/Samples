using Autofac.Extensions.DependencyInjection;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.OpenApi.Models;
using Serilog;
using SimpleApi;

var thisAssembly = typeof(Program).Assembly;
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{environment}.json", true)
    .AddYamlFile("appsettings.yaml", optional: true, reloadOnChange: true)
    .AddYamlFile($"appsettings.{environment}.yaml", optional: true, reloadOnChange: true)
    .Build();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

try
{
    Log.Information("Starting SimpleAPI");
    var builder = WebApplication.CreateBuilder(args);

    // Support YAML settings
    builder.Host.ConfigureAppConfiguration((host, config) =>
    {
        var env = host.HostingEnvironment;
        config.AddYamlFile("appsettings.yaml", optional: true, reloadOnChange: true);
        config.AddYamlFile($"appsettings.{env.EnvironmentName}.yaml", optional: true, reloadOnChange: true);
    });
    
    // Use Autofac
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

    // Wire up Serilog
    builder.Host.UseSerilog();

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
            options.IncludeExceptionDetails = (ctx, ex) =>
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
    app.UseProblemDetails();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.DefaultModelsExpandDepth(-1)); // Disable swagger schemas at bottom

    app.MapControllers();
    app
        .MapGet("/", () => $"Welcome to SimpleAPI\n\n{BuildInformation.Instance.ToDisplayString()}")
        .ExcludeFromDescription();

    app.Run();
}
catch (Exception x)
{
    Log.Error(x, "Unhandled exception: {Exception}", x.Message);
}
finally
{
    Log.Information("SimpleAPI shutdown");
}
