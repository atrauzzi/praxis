using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using Flurl.Http;
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

        public Suite(IConfiguration configuration)
        {
            StaticHost = configuration.GetSection("StaticHost").Value;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await EnsureManifest();

            var suiteName = context
                .AllAttributes
                .FirstOrDefault(a => a.Name == "name")
                .Value as string;

            output.TagName = "script";
            output.Attributes.Add("type", "text/javascript");
            output.Attributes.Add("src", GetVersionedUri(suiteName, ".js"));
            output.TagMode = TagMode.StartTagAndEndTag;

            output.PostElement.AppendHtml($@"<link rel=""stylesheet"" type=""text/css"" href=""{GetVersionedUri(suiteName, "css")}"" />");
        }

        private string GetVersionedUri(string suiteName, string extension)
        {
            var file = $"{AssetPrefix}/{suiteName}.{extension}";
            var versionedFile = _manifest[file] ?? file;

            return $"{AssetPrefix}/{versionedFile}";
        }

        private async Task EnsureManifest()
        {
            if (_manifest == null)
            {
                var manifest = await ManifestUri
                    .AllowAnyHttpStatus()
                    .GetStringAsync();

                _manifest = JObject.Parse(manifest)?
                    .Properties()?
                    .ToDictionary(
                        p => p.Name,
                        p => p.Value.Value<string>());
            }
        }
    }
}
