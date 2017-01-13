using System;
using System.Collections.Generic;

namespace Praxis.Options
{
    // Note: This cannot be an interface because `IOptions` requires an instantiable class with a constructor.
    public class TrustedHosts : Dictionary<string, Uri>
    {
        public const string PublicSite = "PublicSite";
        public const string Static = "Static";

        public Uri PublicSiteUri => this[PublicSite];
        public Uri StaticUri => this[Static];
    }
}
