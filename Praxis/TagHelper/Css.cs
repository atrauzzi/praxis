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
            IEnumerable<IOptions<Manifest>> manifest
        )
        {
            _trustedHosts = trustedHosts.Value;
            _manifests = manifest.Select(m => m.Value);
            _httpContext = httpContextAccessor.HttpContext;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var asset = (context
                .AllAttributes
                .FirstOrDefault(a => a.Name == "name")
                .Value as HtmlString)
                .Value;

            var host = (context
                .AllAttributes
                .FirstOrDefault(a => a.Name == "host")
                .Value as HtmlString)?
                .Value
                ?? TrustedHosts.Static;

            Nonce(asset);

            var uri = BuildUri(asset, host);

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

        private Uri BuildUri(string asset, string host)
        {
            
            var baseUri = _trustedHosts[host];
            var fileName = $"{asset}.css";
            var filePath = _manifests[fileName];
            return new Uri(baseUri, filePath);
        }
    }
}
