using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using ContactManager.API.Middleware;
using ContactManager.API.Swagger;
using ContactManager.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
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
                .AddControllers(options =>
                {
                    options.OutputFormatters.Add(new XmlSerializerOutputFormatterNamespace(new XmlWriterSettings
                    {
                        OmitXmlDeclaration = false
                    }));
                })
                .AddXmlSerializerFormatters()
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

            app.UseSwaggerThemes();
            app.UseSwagger(c => c.SerializeAsV2 = true);
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = "Contact Manager API";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Contact Manager API v1");
                c.InjectStylesheet("/swagger-theme");
            });

            app.UseRequestLogging();
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

    public class XmlSerializerOutputFormatterNamespace : XmlSerializerOutputFormatter
    {
        public XmlSerializerOutputFormatterNamespace(XmlWriterSettings writerSettings) : base(writerSettings) { }
        protected override void Serialize(XmlSerializer xmlSerializer, XmlWriter xmlWriter, object value)
        {
            //applying "empty" namespace will produce no namespaces
            var emptyNamespaces = new XmlSerializerNamespaces();
            emptyNamespaces.Add("", "");
            xmlSerializer.Serialize(xmlWriter, value, emptyNamespaces);
        }
    }
}