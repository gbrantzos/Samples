using System.Collections.Specialized;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuartzHostedService.QuartzScheduler;
using QuartzHostedService.Samples;

namespace QuartzHostedService
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
            => this.configuration = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Setup WebAPI
            services.AddControllers();

            // Settings
            services.Configure<SampleSettings>(configuration.GetSection("SampleSettings"));

            // Add hosted Quartz service
            var props = new NameValueCollection { { "quartz.scheduler.instanceName", "Quartz Sample" } };
            services.AddScheduler();// props, typeof(Startup).Assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
