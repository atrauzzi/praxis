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

            var jsFile = $"{AssetPrefix}/{suiteName}.js";
            var cssFile = $"{AssetPrefix}/{suiteName}.css";

            var versionedJsFile = _manifest[jsFile] ?? jsFile;
            var versionedCssFile = _manifest[cssFile] ?? cssFile;

            var jsUri = $"{AssetPrefix}/{versionedJsFile}";
            var cssUri = $"{AssetPrefix}/{versionedCssFile}";

            output.TagName = "script";
            output.Attributes.Add("type", "text/javascript");
            output.Attributes.Add("src", jsUri);
            output.TagMode = TagMode.StartTagAndEndTag;

            output.PostElement.AppendHtml($@"<link rel=""stylesheet"" type=""text/css"" href=""{cssUri}"" />");
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
