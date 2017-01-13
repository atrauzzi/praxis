using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Praxis.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigurePraxis<T>(this IServiceCollection services, IConfiguration configuration)
        where T : class
        {
            services.Configure<T>(configuration);

            typeof(OptionsConfigurationServiceCollectionExtensions)
                .GetMethods()
                .First(m => m.IsGenericMethod && m.Name.Equals("Configure"))
                .MakeGenericMethod(typeof(T).GetTypeInfo().BaseType)
                .Invoke(null, new object[] { services, configuration });

            return services;
        }
    }
}
