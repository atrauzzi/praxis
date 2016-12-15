using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;

namespace Praxis.Authentication
{
    public class SimpleTicketDataFormat : ISecureDataFormat<AuthenticationTicket>
    {
        public const string ClaimType = "Microsoft.AspNetCore.Authentication.Cookies-SessionId";

        public string Protect(AuthenticationTicket data)
        {
            // ToDo: Also serialize authentication scheme.
            return data
                .Principal?
                .Claims?
                .First(c => c.Type == ClaimType)?
                .Value;
        }

        public string Protect(AuthenticationTicket data, string purpose)
        {
            return Protect(data);
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimType, protectedText));

            return new AuthenticationTicket(
                new ClaimsPrincipal(identity),
                new AuthenticationProperties(),
                // ToDo: Also deserialize authentication scheme.
                "Cookie"
            );
        }

        public AuthenticationTicket Unprotect(string protectedText, string purpose)
        {
            return Unprotect(protectedText);
        }
    }
}
