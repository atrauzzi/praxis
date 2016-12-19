using System.Net.Http;

namespace Praxis.Routing
{
    public struct Route
    {
        public string Name { get; }

        public string Path { get; }

        public HttpMethod Method { get; }

        public string ContentType { get; }

        public Route(
            HttpMethod method,
            string path,
            string name, 
            string contentType = null
        )
        {
            Method = method;
            Path = path;
            Name = name;
            ContentType = contentType;
        }

        public static Route Any(string path, string name, string contentType = null) 
            => new Route(null, path, name, contentType);

        public static Route Get(string path, string name, string contentType = null)
            => new Route(HttpMethod.Get, path, name, contentType);

        public static Route Post(string path, string name, string contentType = null)
            => new Route(HttpMethod.Post, path, name, contentType);

    }
}
