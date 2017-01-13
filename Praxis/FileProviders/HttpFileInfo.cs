using System;
using System.IO;
using Microsoft.Extensions.FileProviders;
using System.Linq;
using Flurl.Http;

namespace Praxis.FileProviders
{
    public class HttpFileInfo : IFileInfo
    {
        public Uri Uri { get; set; }
        public bool Exists { get; set; }
        public long Length { get; set; }
        public string PhysicalPath { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public bool IsDirectory => false; // Important so as to not trigger directory contents behaviour.

        public string Name => Uri.Segments.Last();

        public Stream CreateReadStream()
        {
            var request = Uri
                .ToString()
                .GetStreamAsync();

            request.Wait();

            return request.Result;
        }

    }
}
