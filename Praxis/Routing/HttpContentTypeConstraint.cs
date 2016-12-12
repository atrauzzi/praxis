using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Praxis.Routing
{
    public class HttpContentTypeConstraint : IRouteConstraint
    {
        private readonly string _contentType;

        public HttpContentTypeConstraint(string contentType)
        {
            _contentType = contentType;
        }

        public bool Match(
            HttpContext httpContext,
            IRouter route,
            string routeKey,
            RouteValueDictionary values,
            RouteDirection routeDirection
        )
        {
            var requestContentType = httpContext.Request.ContentType;

            return 
                !string.IsNullOrEmpty(requestContentType) 
                && requestContentType.Equals(_contentType);
        }
    }
}
