using ContactManager.Domain.Model;
using Microsoft.Extensions.DependencyInjection;

namespace ContactManager.Domain
{
    public static class Extensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IContactRepository, ContactRepository>();

            return services;
        }
    }
}
