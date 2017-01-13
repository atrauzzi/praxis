using System;
using System.Collections.Generic;

namespace Praxis.Options
{
    // Note: This class is designed to be subclassed in your project.  You can then optionally override `PathPrefix`
    //       or use it as-is.
    //       To help in your templates, you can also optionally add `string` constants to your subclass representing 
    //       the various base filenames you expect to see in your manifest!  Yay strongly typed assets!
    //
    // Note: This cannot be an interface or abstract because `IOptions` requires an instantiable class with a constructor.
    public class Manifest : Dictionary<string, Uri>
    {
        public virtual string Name => "Default";

        public virtual string Host => TrustedHosts.Static;

        public virtual Uri PathPrefix => new Uri("/", UriKind.Relative);

        public Uri BuildAssetUri(TrustedHosts trustedHosts, string assetName)
        {
            var baseUri = new Uri(trustedHosts[Host], PathPrefix);
            var filePath = this[assetName];

            return new Uri($"{baseUri}/{filePath}");
        }
    }
}
