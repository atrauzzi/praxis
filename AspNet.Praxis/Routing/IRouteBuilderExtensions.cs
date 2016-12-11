using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;

namespace Praxis.Routing
{
    public static class IRouteBuilderExtensions
    {
        public static IRouteBuilder Any(
            this IRouteBuilder routes,
            string path,
            string controller,
            string action,
            string name,
            string contentType = null,
            IDictionary<string, object> constraints = null
        )
        {
            return routes.AddVerb(null, path, controller, action, name, contentType, constraints);
        }

        public static IRouteBuilder Get(
            this IRouteBuilder routes,
            string path,
            string controller,
            string action,
            string name,
            string contentType = null,
            IDictionary<string, object> constraints = null
        )
        {
            return routes.AddVerb("GET", path, controller, action, name, contentType, constraints);
        }

        public static IRouteBuilder Post(
            this IRouteBuilder routes,
            string path,
            string controller,
            string action,
            string name,
            string contentType = null,
            IDictionary<string, object> constraints = null
        )
        {
            return routes.AddVerb("POST", path, controller, action, name, contentType, constraints);
        }

        private static IRouteBuilder AddVerb(
            this IRouteBuilder routes,
            string httpMethod,
            string path,
            string controller,
            string action,
            string name,
            string contentType = null,
            IDictionary<string, object> constraints = null,
            object defaults = null
        )
        {
            if (defaults == null)
                defaults = new
                {
                    controller,
                    action,
                };

            if (constraints == null)
                constraints = new Dictionary<string, object>();

            if (httpMethod != null)
                constraints.Add("httpMethod", new HttpMethodRouteConstraint(httpMethod));

            if (contentType != null)
                constraints.Add("contentType", new HttpContentTypeConstraint(contentType));

            return routes.MapRoute(name, path, defaults, new RouteValueDictionary(constraints));
        }
    }
}