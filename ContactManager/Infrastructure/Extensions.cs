using ContactManager.Domain.Core;
using ContactManager.Domain.Model;
using ContactManager.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContactManager.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("EventStore");

            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddSingleton<IEventStore>(new SqlServerEventStore(connectionString));

            return services;
        }
    }
}
