using System;
using System.IO;
using System.Reflection;
using ContactManager.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace ContactManager.API
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration) => this.configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            var thisAssembly = typeof(Startup).Assembly;

            services
                .AddControllers()
                .AddNewtonsoftJson(options => options.UseCamelCasing(true));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Contact Manager API", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddInfrastructureServices(configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseSwagger(c => c.SerializeAsV2 = true);
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = "Contact Manager API";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Contact Manager API v1");
                // c.InjectStylesheet("/swagger-theme");
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                // Promote Swagger as home page
                endpoints.MapGet("/", async context =>
                {
                    context.Response.Redirect("/swagger");
                    await context.Response.CompleteAsync();
                });
            });
        }
    }
}