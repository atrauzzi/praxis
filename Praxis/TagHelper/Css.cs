using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Praxis.Options;
using System.Linq;

namespace Praxis.TagHelper
{
    public class Css : Microsoft.AspNetCore.Razor.TagHelpers.TagHelper
    {
        private const string RootCssNonceKey = "praxis:css:nonce";

        private readonly IEnumerable<Manifest> _manifests;

        private readonly TrustedHosts _trustedHosts;

        private readonly HttpContext _httpContext;

        public Css(
            IHttpContextAccessor httpContextAccessor, 
            IOptions<TrustedHosts> trustedHosts,
            IEnumerable<Manifest> manifests
        )
        {
            _trustedHosts = trustedHosts.Value;
            _manifests = manifests;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var assetName = (context
                .AllAttributes
                .FirstOrDefault(a => a.Name == "name")
                .Value as HtmlString)
                .Value;

            var manifestName = (context
                .AllAttributes
                .FirstOrDefault(a => a.Name == "manifest")
                .Value as HtmlString)?
                .Value
                ?? TrustedHosts.Static;

            Nonce(assetName);

            var fileName = $"{assetName}.css";
            var manifest = _manifests.First(m => 
                m?.Name.Equals(manifestName) ?? false);
            var uri = manifest.BuildAssetUri(_trustedHosts, fileName);

            output.TagName = "link";
            output.TagMode = TagMode.SelfClosing;
            output.Attributes.Clear();
            output.Attributes.Add("rel", "stylesheet");
            output.Attributes.Add("type", "text/css");
            output.Attributes.Add("href", uri.ToString());

            await Task.FromResult(0);
        }

        private void Nonce(string name)
        {
            if (_httpContext.Items.ContainsKey(RootCssNonceKey)) {
                var priorAsset = _httpContext.Items[RootCssNonceKey];
                throw new Exception($"A stylesheet tag ({priorAsset}) has already been rendered for the current request.");
            }

            _httpContext.Items.Add(RootCssNonceKey, name);
        }
    }
}
