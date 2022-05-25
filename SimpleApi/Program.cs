using Autofac.Extensions.DependencyInjection;
using MediatR;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Templates;
using Serilog.Templates.Themes;
using SimpleApi;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(new ExpressionTemplate(
        "[{@l:u4}]{#if @p['TraceID'] is not null}[{@p['TraceID']}]{#end} {@m}\n{@x}",
        theme: TemplateTheme.Literate))
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .CreateLogger();

try
{
    Log.Information("Starting SimpleAPI");

    var builder = WebApplication.CreateBuilder(args);

    // Use Autofac
    builder.Host
        .UseServiceProviderFactory(new AutofacServiceProviderFactory());

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
        .AddMediatR(typeof(Program).Assembly)
        .AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));

    // Other services
    builder.Services.AddHttpContextAccessor();

    // Build app and run
    var app = builder.Build();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.DefaultModelsExpandDepth(-1)); // Disable swagger schemas at bottom
    app.MapControllers();


    app.MapGet("/", () => "Welcome to SimpleAPI!").ExcludeFromDescription();
    ;
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
