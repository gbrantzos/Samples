using Autofac.Extensions.DependencyInjection;
using MediatR;
using Serilog;
using Serilog.Templates;
using Serilog.Templates.Themes;
using SimpleApi;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(new ExpressionTemplate("[{@l:w4}] {@m} {Rest(true)}\n{@x}", theme:TemplateTheme.Literate))
    .MinimumLevel.Debug()
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
    
    // Setup Mediator
    builder.Services.AddMediatR(typeof(Program).Assembly);
    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));

    // Other services
    builder.Services.AddHttpContextAccessor();
    
    // Build app and run
    var app = builder.Build();

    app.MapGet("/",
        async (ctx) =>
        {
            var mediator = ctx.RequestServices.GetRequiredService<IMediator>();
            var list = await mediator.Send(new SearchTodos());
            await ctx.Response.WriteAsync($"Hello World, todos count {list.DataOrDefault()!.Count}");
        });

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
