using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using Flurl.Http;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Praxis.TagHelper
{
    public class Suite : Microsoft.AspNetCore.Razor.TagHelpers.TagHelper
    {
        private const string ManifestFile = "rev-manifest.json";

        private const string AssetPath = "lib";

        private static IDictionary<string, string> _manifest;

        private string StaticHost { get; }

        private string AssetPrefix => $"{StaticHost}/{AssetPath}";

        private string ManifestUri => $"{AssetPrefix}/{ManifestFile}";

        public Suite(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            var httpContext = httpContextAccessor.HttpContext;
            var requestId = httpContext.TraceIdentifier;

            if (httpContext.Items.ContainsKey(requestId))
            {
                throw new Exception("A suite has already been rendered for this page.");
            }

            httpContext.Items.Add(requestId, true);

            StaticHost = configuration.GetSection("StaticHost").Value;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await EnsureManifest();

            var suiteName = (context
                .AllAttributes
                .FirstOrDefault(a => a.Name == "name")
                .Value as HtmlString)
                .Value;

            output.TagName = "script";
            output.Attributes.Clear();
            output.Attributes.Add("type", "text/javascript");
            output.Attributes.Add("src", GetVersionedUri(suiteName, "js"));
            output.TagMode = TagMode.StartTagAndEndTag;

            output.PostElement.AppendHtml($@"<link rel=""stylesheet"" type=""text/css"" href=""{GetVersionedUri(suiteName, "css")}"" />");
        }

        private string GetVersionedUri(string suiteName, string extension)
        {
            var file = $"{suiteName}.{extension}";
            var versionedFile = _manifest
                .FirstOrDefault(info => info.Key == file)
                .Value ?? file;

            return $"{AssetPrefix}/{versionedFile}";
        }

        private async Task EnsureManifest()
        {
            if (_manifest == null)
            {
                var manifestResponse = await ManifestUri
                    .AllowAnyHttpStatus()
                    .GetAsync();

                if (manifestResponse.IsSuccessStatusCode)
                {
                    var manifestJson = await manifestResponse.Content.ReadAsStringAsync();

                    _manifest = JsonConvert
                        .DeserializeObject<JObject>(manifestJson)?
                        .Properties()?
                        .ToDictionary(
                            p => p.Name,
                            p => p.Value.Value<string>());
                }
                else
                {
                    _manifest = new Dictionary<string, string>();
                }
            }
        }
    }
}
