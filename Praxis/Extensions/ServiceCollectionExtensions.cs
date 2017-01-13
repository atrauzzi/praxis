using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Praxis.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private static readonly IList<Type> _options = new List<Type>();

        public static IServiceCollection ConfigurePraxis<T1, T2>(this IServiceCollection services, IConfiguration configuration)
        where T1 : class, new()
        where T2 : class, T1, new()
        {
            _options.Add(typeof(IOptions<T2>));
            services.Configure<T2>(configuration);

            services.AddSingleton<IEnumerable<T1>>(provider => _options
                .Select(t => 
                    (provider.GetService(t) as IOptions<T2>)?.Value));

            return services;
        }
    }
}
