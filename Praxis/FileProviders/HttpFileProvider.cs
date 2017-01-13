using System;
using Flurl.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.Linq;

namespace Praxis.FileProviders
{
    public class HttpFileProvider : IFileProvider
    {
        private readonly Uri _baseUri;

        public HttpFileProvider(Uri baseUri)
        {
            _baseUri = baseUri;
        }

        private Uri BuildUri(string subpath)
        {
            // Note: For some reason, doing it this way seems to overwrite any existing prefix.
            // return new Uri(_baseUri, new Uri(subpath, UriKind.Relative));
            return new Uri($"{_baseUri}/{subpath}");
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            var uri = BuildUri(subpath);

            var httpInfoRequest = uri.ToString().HeadAsync();

            httpInfoRequest.Wait();

            var response = httpInfoRequest.Result;

            var lastModifiedValue = response
                .Headers
                .FirstOrDefault(h => h.Key.ToLower().Equals("last-modified"))
                .Value?
                .FirstOrDefault();

            var contentLengthValue = response
                .Headers
                .FirstOrDefault(h => h.Key.ToLower().Equals("content-length"))
                .Value?
                .FirstOrDefault();

            long contentLength;
            long.TryParse(contentLengthValue, out contentLength);
            DateTime lastModified;
            DateTime.TryParse(lastModifiedValue, out lastModified);

            return new HttpFileInfo
            {
                Uri = uri,
                Exists = response.IsSuccessStatusCode,
                LastModified = lastModified,
                Length = contentLength
            };
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            throw new NotImplementedException();
        }

        public IChangeToken Watch(string filter)
        {
            throw new NotImplementedException();
        }
    }
}
