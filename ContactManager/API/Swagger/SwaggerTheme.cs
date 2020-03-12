using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ContactManager.API.Swagger
{
    public static class SwaggerTheme
    {
        private static Assembly assembly = typeof(SwaggerTheme).Assembly;

        public static string FlatTop => GetTheme("flattop");

        private static string GetTheme(string name)
        {
            var resource = assembly.GetManifestResourceStream($"{typeof(SwaggerTheme).Namespace}.theme-{name}.css");
            using var reader = new StreamReader(resource);

            return reader.ReadToEnd();
        }
    }

    public static class SwaggerThemeExtensions
    {
        public static IApplicationBuilder UseSwaggerThemes(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.Map("/swagger-theme", app =>
            {
                app.Run(async context => await context.Response.WriteAsync(SwaggerTheme.FlatTop));
            });
            return applicationBuilder;
        }
    }
}
