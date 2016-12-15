using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.DependencyInjection;

namespace Praxis.Session.Json
{
    public static class JsonSessionServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonSession(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddTransient<ISessionStore, DistributedJsonSessionStore>();

            return services;
        }

        public static IServiceCollection AddJsonSession(this IServiceCollection services, Action<SessionOptions> configure)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            services.Configure(configure);
            services.AddJsonSession();

            return services;
        }
    }
}